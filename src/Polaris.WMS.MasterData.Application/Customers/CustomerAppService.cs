using Polaris.WMS.MasterData.Application.Contracts.Customers;
using Polaris.WMS.MasterData.Domain.Customers;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Customers;

public class CustomerAppService(IRepository<Customer, Guid> customerRepository)
    : ApplicationService, ICustomerAppService
{
    /// <inheritdoc />
    public async Task<CustomerDto> CreateAsync(CreateUpdateCustomerDto input)
    {
        var normalizedCode = input.Code.Trim();
        var exists = await customerRepository.AnyAsync(x => x.Code == normalizedCode);
        if (exists)
        {
            throw new BusinessException("客户编码已存在。")
                .WithData("Code", normalizedCode);
        }

        var entity = new Customer(
            GuidGenerator.Create(),
            normalizedCode,
            input.Name,
            input.ContactName,
            input.Phone,
            input.Address,
            input.IsEnabled,
            input.Remark);

        await customerRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<CustomerDto> UpdateAsync(Guid id, CreateUpdateCustomerDto input)
    {
        var entity = await customerRepository.GetAsync(id);
        var normalizedCode = input.Code.Trim();
        var exists = await customerRepository.AnyAsync(x => x.Id != id && x.Code == normalizedCode);
        if (exists)
        {
            throw new BusinessException("客户编码已存在。")
                .WithData("Code", normalizedCode);
        }

        entity.Update(normalizedCode, input.Name, input.ContactName, input.Phone, input.Address, input.IsEnabled, input.Remark);
        await customerRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<CustomerDto> GetAsync(Guid id)
    {
        var entity = await customerRepository.GetAsync(id);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<CustomerDto>> GetListAsync(CustomerSearchDto input)
    {
        var query = await customerRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Code))
        {
            query = query.Where(x => x.Code.Contains(input.Code));
        }

        if (!string.IsNullOrWhiteSpace(input.Name))
        {
            query = query.Where(x => x.Name.Contains(input.Name));
        }

        if (input.IsEnabled.HasValue)
        {
            query = query.Where(x => x.IsEnabled == input.IsEnabled.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderBy(x => x.Code).PageBy(input.SkipCount, input.MaxResultCount);
        var entities = await AsyncExecuter.ToListAsync(query);
        var items = entities.Select(MapToDto).ToList();
        return new PagedResultDto<CustomerDto>(totalCount, items);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await customerRepository.DeleteAsync(id);
    }

    private static CustomerDto MapToDto(Customer source)
    {
        return new CustomerDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            ContactName = source.ContactName,
            Phone = source.Phone,
            Address = source.Address,
            IsEnabled = source.IsEnabled,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId
        };
    }
}


