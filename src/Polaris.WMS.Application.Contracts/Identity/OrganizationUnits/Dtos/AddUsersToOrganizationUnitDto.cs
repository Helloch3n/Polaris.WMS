using System;
using System.Collections.Generic;

namespace Polaris.WMS.Identity.OrganizationUnits.Dtos
{
    public class AddUsersToOrganizationUnitDto
    {
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
}
