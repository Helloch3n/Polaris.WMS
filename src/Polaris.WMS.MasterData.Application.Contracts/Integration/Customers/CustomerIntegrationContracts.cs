using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Integration.Customers;

public class CustomerIntegrationDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsEnabled { get; set; }
}

[RemoteService(IsEnabled = false)]
public interface ICustomerIntegrationService : IApplicationService
{
    Task<CustomerIntegrationDto> GetAsync(Guid id);
    Task<CustomerIntegrationDto> GetCustomerInfoByCodeAsync(string customerCode);
}

