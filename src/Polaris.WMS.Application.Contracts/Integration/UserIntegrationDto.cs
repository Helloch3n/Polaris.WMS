using System;

namespace Polaris.WMS.Integration;

public class UserIntegrationDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}