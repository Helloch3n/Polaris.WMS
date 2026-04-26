using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData.Domain.Customers;

public class Customer : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? ContactName { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public bool IsEnabled { get; private set; }
    public string? Remark { get; private set; }

    protected Customer()
    {
        Code = string.Empty;
        Name = string.Empty;
        IsEnabled = true;
    }

    public Customer(
        Guid id,
        string code,
        string name,
        string? contactName,
        string? phone,
        string? address,
        bool isEnabled,
        string? remark = null) : base(id)
    {
        Update(code, name, contactName, phone, address, isEnabled, remark);
    }

    public void Update(
        string code,
        string name,
        string? contactName,
        string? phone,
        string? address,
        bool isEnabled,
        string? remark = null)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 50);
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
        ContactName = contactName?.Trim();
        Phone = phone?.Trim();
        Address = address?.Trim();
        IsEnabled = isEnabled;
        Remark = remark?.Trim();
    }
}

