using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Suppliers;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Integration.Suppliers;

public class SupplierIntegrationService(
    IRepository<Supplier,Guid> supplierRepository):ApplicationService,ISupplierIntegrationService
{
    public async Task<SupplierIntegrationDto> GetSupplierInfoByCodeAsync(string supplierCode)
    {
        var supplier = await supplierRepository.FirstOrDefaultAsync(x => x.Code == supplierCode);
        if (supplier == null)
        {
            throw new UserFriendlyException($"主数据异常：WMS 供应商列表中不存在代码为 [{supplierCode}] 的供应商。请先同步主数据！");
        }

        return new SupplierIntegrationDto { Id = supplier.Id, Name = supplier.Name, Code = supplier.Code };
    }
}