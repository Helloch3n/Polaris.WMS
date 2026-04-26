using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Customers;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.Outbound.Application.Contracts.SalesShipments;
using Polaris.WMS.Outbound.Domain.SalesShipments;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Outbound.Application.SalesShipments;

public class SalesShipmentAppService(
    IRepository<SalesShipment, Guid> salesShipmentRepository,
    IRepository<SalesShipmentRecord, Guid> salesShipmentRecordRepository,
    IRepository<SalesShipmentAllocation, Guid> salesShipmentAllocationRepository,
    SalesShipmentManager salesShipmentManager,
    ICustomerIntegrationService customerIntegrationService,
    IContainerIntegrationService containerIntegrationService,
    ILocationIntegrationService locationIntegrationService)
    : ApplicationService, ISalesShipmentAppService
{
    /// <inheritdoc />
    public async Task<SalesShipmentDto> CreateAsync(CreateSalesShipmentDto input)
    {
        var customer = await customerIntegrationService.GetAsync(input.CustomerId);
        if (!customer.IsEnabled)
        {
            throw new BusinessException("客户已停用，不能创建销售发货单。")
                .WithData("CustomerId", input.CustomerId);
        }

        var exists = await salesShipmentRepository.AnyAsync(x => x.ShipmentNo == input.ShipmentNo);
        if (exists)
        {
            throw new BusinessException("销售发货单号已存在。")
                .WithData("ShipmentNo", input.ShipmentNo);
        }

        var entity = SalesShipment.Create(
            GuidGenerator.Create(),
            input.ShipmentNo,
            input.SourceSalesOrderId,
            input.SourceSalesOrderNo,
            customer.Id,
            customer.Code,
            customer.Name,
            input.DeliveryContactName ?? customer.ContactName,
            input.DeliveryPhone ?? customer.Phone,
            input.DeliveryAddress ?? customer.Address,
            input.Remark);

        foreach (var detail in input.Details)
        {
            entity.AddDetail(
                GuidGenerator.Create(),
                detail.SourceSalesOrderLineId,
                detail.LineNo,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.Unit,
                detail.RequiredQty,
                detail.Remark);
        }

        await salesShipmentRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<SalesShipmentDto> UpdateAsync(Guid id, UpdateSalesShipmentDto input)
    {
        var query = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(SalesShipment), id);
        }

        var customer = await customerIntegrationService.GetAsync(input.CustomerId);
        if (!customer.IsEnabled)
        {
            throw new BusinessException("客户已停用，不能更新销售发货单。")
                .WithData("CustomerId", input.CustomerId);
        }

        entity.UpdateHeader(
            customer.Id,
            customer.Code,
            customer.Name,
            input.DeliveryContactName ?? customer.ContactName,
            input.DeliveryPhone ?? customer.Phone,
            input.DeliveryAddress ?? customer.Address,
            input.Remark);

        foreach (var detail in entity.Details.ToList())
        {
            entity.RemoveDetail(detail.Id);
        }

        foreach (var detail in input.Details)
        {
            entity.AddDetail(
                GuidGenerator.Create(),
                detail.SourceSalesOrderLineId,
                detail.LineNo,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.Unit,
                detail.RequiredQty,
                detail.Remark);
        }

        await salesShipmentRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<SalesShipmentDto> GetAsync(Guid id)
    {
        var query = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(SalesShipment), id);
        }

        return await BuildShipmentDtoAsync(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<SalesShipmentDto>> GetListAsync(SalesShipmentSearchDto input)
    {
        var query = await salesShipmentRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.ShipmentNo))
        {
            query = query.Where(x => x.ShipmentNo.Contains(input.ShipmentNo));
        }

        if (!string.IsNullOrWhiteSpace(input.CustomerCode))
        {
            query = query.Where(x => x.CustomerCode.Contains(input.CustomerCode));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderByDescending(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount);
        var entities = await AsyncExecuter.ToListAsync(query);
        var items = entities.Select(MapToDtoWithoutDetails).ToList();
        return new PagedResultDto<SalesShipmentDto>(totalCount, items);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await salesShipmentRepository.DeleteAsync(id);
    }

    /// <inheritdoc />
    public async Task<SalesShipmentAllocationDto> AddAllocationAsync(CreateSalesShipmentAllocationDto input)
    {
        if (input.SalesShipmentId == Guid.Empty)
        {
            throw new BusinessException("销售发货单Id不能为空。");
        }

        if (input.DetailId == Guid.Empty)
        {
            throw new BusinessException("销售发货明细Id不能为空。");
        }

        var containerCode = input.ContainerCode?.Trim();
        if (string.IsNullOrWhiteSpace(containerCode))
        {
            throw new BusinessException("容器编码不能为空。");
        }

        var locationCode = input.LocationCode?.Trim();
        if (string.IsNullOrWhiteSpace(locationCode))
        {
            throw new BusinessException("库位编码不能为空。");
        }

        var container = await containerIntegrationService.GetContainerByCodeAsync(containerCode);
        if (container == null)
        {
            throw new BusinessException("系统不存在对应的容器编码。")
                .WithData("ContainerCode", containerCode);
        }

        var location = await locationIntegrationService.GetLocationInfoByCodeAsync(locationCode);

        var entity = await salesShipmentManager.AddAllocationAsync(
            input.SalesShipmentId,
            input.DetailId,
            new SalesShipmentAllocationCreateInfo
            {
                Id = GuidGenerator.Create(),
                Qty = input.Qty,
                ContainerId = container.Id,
                ContainerCode = containerCode,
                SourceLocationId = location.Id,
                SourceLocationCode = locationCode,
                BatchNo = input.BatchNo,
                SN = input.SN
            });

        return MapToAllocationDto(entity);
    }

    /// <inheritdoc />
    public async Task RemoveAllocationAsync(Guid salesShipmentId, Guid detailId, Guid allocationId)
    {
        if (salesShipmentId == Guid.Empty)
        {
            throw new BusinessException("销售发货单Id不能为空。");
        }

        if (detailId == Guid.Empty)
        {
            throw new BusinessException("销售发货明细Id不能为空。");
        }

        if (allocationId == Guid.Empty)
        {
            throw new BusinessException("分配记录Id不能为空。");
        }

        await salesShipmentManager.RemoveAllocationAsync(salesShipmentId, detailId, allocationId);
    }

    /// <inheritdoc />
    public async Task<SalesShipmentAllocationDto> MarkAllocationPickedAsync(MarkSalesShipmentAllocationPickedDto input)
    {
        if (input.SalesShipmentId == Guid.Empty)
        {
            throw new BusinessException("销售发货单Id不能为空。");
        }

        if (input.DetailId == Guid.Empty)
        {
            throw new BusinessException("销售发货明细Id不能为空。");
        }

        if (input.AllocationId == Guid.Empty)
        {
            throw new BusinessException("分配记录Id不能为空。");
        }

        var entity = await salesShipmentManager.MarkAllocationPickedAsync(input.SalesShipmentId, input.DetailId, input.AllocationId);
        return MapToAllocationDto(entity);
    }

    /// <inheritdoc />
    public async Task<SalesShipmentRecordDto> AddRecordsAsync(AddSalesShipmentRecordsDto input)
    {
        if (input.SalesShipmentId == Guid.Empty)
        {
            throw new BusinessException("销售发货单Id不能为空。");
        }

        if (input.DetailId == Guid.Empty)
        {
            throw new BusinessException("销售发货明细Id不能为空。");
        }

        if (input.Records.Count != 1)
        {
            throw new BusinessException("一次仅允许新增一条发货记录。");
        }

        var recordInfos = new List<SalesShipmentRecordCreateInfo>(input.Records.Count);
        foreach (var record in input.Records)
        {
            var containerCode = record.ContainerCode?.Trim();
            if (string.IsNullOrWhiteSpace(containerCode))
            {
                throw new BusinessException("容器编码不能为空。");
            }

            var locationCode = record.LocationCode?.Trim();
            if (string.IsNullOrWhiteSpace(locationCode))
            {
                throw new BusinessException("库位编码不能为空。");
            }

            var container = await containerIntegrationService.GetContainerByCodeAsync(containerCode);
            if (container == null)
            {
                throw new BusinessException("系统不存在对应的容器编码。")
                    .WithData("ContainerCode", containerCode);
            }

            var location = await locationIntegrationService.GetLocationInfoByCodeAsync(locationCode);

            recordInfos.Add(new SalesShipmentRecordCreateInfo
            {
                Id = GuidGenerator.Create(),
                Qty = record.Qty,
                ContainerId = container.Id,
                ContainerCode = containerCode,
                LocationId = location.Id,
                LocationCode = locationCode,
                BatchNo = record.BatchNo,
                SN = record.SN
            });
        }

        // 第一步：调用领域服务追加发货记录并完成业务校验
        await salesShipmentManager.AddRecordsAsync(input.SalesShipmentId, input.DetailId, recordInfos);

        // 第二步：返回新增记录
        var createdRecordId = recordInfos[0].Id;
        var createdRecord = await salesShipmentRecordRepository.FirstOrDefaultAsync(x => x.Id == createdRecordId);
        if (createdRecord == null)
        {
            throw new EntityNotFoundException(typeof(SalesShipmentRecord), createdRecordId);
        }

        return MapToRecordDto(createdRecord);
    }

    /// <inheritdoc />
    public async Task ApproveAndExecuteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("销售发货单Id不能为空。");
        }

        await salesShipmentManager.ApproveAndExecuteAsync(id);
    }

    private async Task<SalesShipmentDto> BuildShipmentDtoAsync(SalesShipment shipment)
    {
        var dto = MapToDto(shipment);
        var allocations = await salesShipmentAllocationRepository.GetListAsync(x => x.SalesShipmentId == shipment.Id);
        var allocationLookup = allocations
            .GroupBy(x => x.SalesShipmentDetailId)
            .ToDictionary(x => x.Key, x => x.Select(MapToAllocationDto).ToList());

        var records = await salesShipmentRecordRepository.GetListAsync(x => x.SalesShipmentId == shipment.Id);
        var recordLookup = records
            .GroupBy(x => x.SalesShipmentDetailId)
            .ToDictionary(x => x.Key, x => x.Select(MapToRecordDto).ToList());

        foreach (var detail in dto.Details)
        {
            detail.Allocations = allocationLookup.GetValueOrDefault(detail.Id, new List<SalesShipmentAllocationDto>());
            detail.Records = recordLookup.GetValueOrDefault(detail.Id, new List<SalesShipmentRecordDto>());
        }

        return dto;
    }

    private static SalesShipmentDto MapToDto(SalesShipment source)
    {
        var dto = MapToDtoWithoutDetails(source);
        dto.Details = source.Details.Select(x => new SalesShipmentDetailDto
        {
            Id = x.Id,
            SalesShipmentId = x.SalesShipmentId,
            SourceSalesOrderLineId = x.SourceSalesOrderLineId,
            LineNo = x.LineNo,
            ProductId = x.ProductId,
            ProductCode = x.ProductCode,
            ProductName = x.ProductName,
            Unit = x.Unit,
            RequiredQty = x.RequiredQty,
            AllocatedQty = x.AllocatedQty,
            PickedQty = x.PickedQty,
            ShippedQty = x.ShippedQty,
            Remark = x.Remark,
            Allocations = new List<SalesShipmentAllocationDto>(),
            Records = new List<SalesShipmentRecordDto>()
        }).ToList();
        return dto;
    }

    private static SalesShipmentDto MapToDtoWithoutDetails(SalesShipment source)
    {
        return new SalesShipmentDto
        {
            Id = source.Id,
            ShipmentNo = source.ShipmentNo,
            SourceSalesOrderId = source.SourceSalesOrderId,
            SourceSalesOrderNo = source.SourceSalesOrderNo,
            CustomerId = source.CustomerId,
            CustomerCode = source.CustomerCode,
            CustomerName = source.CustomerName,
            Status = source.Status,
            DeliveryContactName = source.DeliveryContactName,
            DeliveryPhone = source.DeliveryPhone,
            DeliveryAddress = source.DeliveryAddress,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Details = new List<SalesShipmentDetailDto>()
        };
    }

    private static SalesShipmentAllocationDto MapToAllocationDto(SalesShipmentAllocation source)
    {
        return new SalesShipmentAllocationDto
        {
            Id = source.Id,
            SalesShipmentId = source.SalesShipmentId,
            SalesShipmentDetailId = source.SalesShipmentDetailId,
            ProductId = source.ProductId,
            ProductCode = source.ProductCode,
            ProductName = source.ProductName,
            Qty = source.Qty,
            ContainerId = source.ContainerId,
            ContainerCode = source.ContainerCode,
            SourceLocationId = source.SourceLocationId,
            SourceLocationCode = source.SourceLocationCode,
            BatchNo = source.BatchNo,
            SN = source.SN,
            Status = source.Status,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId
        };
    }

    private static SalesShipmentRecordDto MapToRecordDto(SalesShipmentRecord source)
    {
        return new SalesShipmentRecordDto
        {
            Id = source.Id,
            SalesShipmentId = source.SalesShipmentId,
            SalesShipmentDetailId = source.SalesShipmentDetailId,
            SourceSalesOrderLineId = source.SourceSalesOrderLineId,
            ProductId = source.ProductId,
            ProductCode = source.ProductCode,
            ProductName = source.ProductName,
            Qty = source.Qty,
            ContainerId = source.ContainerId,
            ContainerCode = source.ContainerCode,
            LocationId = source.LocationId,
            LocationCode = source.LocationCode,
            BatchNo = source.BatchNo,
            SN = source.SN
        };
    }
}



