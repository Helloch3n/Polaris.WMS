using System;

namespace Polaris.WMS.Integration.Departments;

public class DepartmentIntegrationDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string Code { get; set; }
}