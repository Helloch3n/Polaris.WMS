using Polaris.WMS.MasterData.Application.Contracts.Integration.Customers;
using Polaris.WMS.MasterData.Domain.Customers;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Integration.Customers;

[RemoteService(IsEnabled = false)]
public class CustomerIntegrationService(IRepository<Customer, Guid> customerRepository)
    : ApplicationService, ICustomerIntegrationService
{
    public async Task<CustomerIntegrationDto> GetAsync(Guid id)
    {
        var customer = await customerRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (customer == null)
        {
            throw new UserFriendlyException($"主数据异常：WMS 客户列表中不存在 Id 为 [{id}] 的客户。请先同步主数据！");
        }

        return MapToDto(customer);
    }

    public async Task<CustomerIntegrationDto> GetCustomerInfoByCodeAsync(string customerCode)
    {
        var normalizedCode = customerCode?.Trim();
        var customer = await customerRepository.FirstOrDefaultAsync(x => x.Code == normalizedCode);
        if (customer == null)
        {
            throw new UserFriendlyException($"主数据异常：WMS 客户列表中不存在代码为 [{customerCode}] 的客户。请先同步主数据！");
        }

        return MapToDto(customer);
    }

    private static CustomerIntegrationDto MapToDto(Customer source)
    {
        return new CustomerIntegrationDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            ContactName = source.ContactName,
            Phone = source.Phone,
            Address = source.Address,
            IsEnabled = source.IsEnabled
        };
    }
}

