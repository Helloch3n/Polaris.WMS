using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Suppliers
{
    public interface ISupplierAppService :
        ICrudAppService<
            SupplierDto,
            Guid,
            SupplierSearchDto,
            CreateUpdateSupplierDto>
    {
        Task<PagedResultDto<SupplierDto>> GetListAsync(SupplierSearchDto input);
    }
}
