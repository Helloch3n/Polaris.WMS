using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Customers;

public interface ICustomerAppService : IApplicationService
{
    /// <summary>
    /// 创建客户。
    /// </summary>
    Task<CustomerDto> CreateAsync(CreateUpdateCustomerDto input);

    /// <summary>
    /// 更新客户。
    /// </summary>
    Task<CustomerDto> UpdateAsync(Guid id, CreateUpdateCustomerDto input);

    /// <summary>
    /// 获取客户详情。
    /// </summary>
    Task<CustomerDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取客户列表。
    /// </summary>
    Task<PagedResultDto<CustomerDto>> GetListAsync(CustomerSearchDto input);

    /// <summary>
    /// 删除客户。
    /// </summary>
    Task DeleteAsync(Guid id);
}

