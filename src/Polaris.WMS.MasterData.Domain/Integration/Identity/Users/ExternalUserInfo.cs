namespace Polaris.WMS.MasterData.Domain.Integration.Identity.Users;

public class ExternalUserInfo
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }
}