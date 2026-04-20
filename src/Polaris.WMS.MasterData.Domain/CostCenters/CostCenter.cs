using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData.Domain.CostCenters;

public class CostCenter : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string DepartmentCode { get; private set; }
    public string DepartmentName { get; private set; }
    public string CompanyCode { get; private set; }

    protected CostCenter()
    {
        Code = string.Empty;
        Name = string.Empty;
        DepartmentCode = string.Empty;
        DepartmentName = string.Empty;
        CompanyCode = string.Empty;
    }

    internal CostCenter(
        Guid id,
        string code,
        string name,
        string departmentCode,
        string departmentName,
        string companyCode) : base(id)
    {
        SetCode(code);
        SetName(name);
        SetDepartmentCode(departmentCode);
        SetDepartmentName(departmentName);
        SetCompanyCode(companyCode);
    }

    public void SetCode(string code)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code));
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
    }

    public void SetDepartmentCode(string departmentCode)
    {
        DepartmentCode = Check.NotNullOrWhiteSpace(departmentCode, nameof(departmentCode));
    }

    public void SetDepartmentName(string departmentName)
    {
        DepartmentName = Check.NotNullOrWhiteSpace(departmentName, nameof(departmentName));
    }

    public void SetCompanyCode(string companyCode)
    {
        CompanyCode = Check.NotNullOrWhiteSpace(companyCode, nameof(companyCode));
    }

    public void Update(
        string code,
        string name,
        string departmentCode,
        string departmentName,
        string companyCode)
    {
        SetCode(code);
        SetName(name);
        SetDepartmentCode(departmentCode);
        SetDepartmentName(departmentName);
        SetCompanyCode(companyCode);
    }
}

