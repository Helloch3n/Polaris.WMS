using System;
using System.Collections.Generic;

namespace Polaris.WMS.Users.Dtos
{
    public class AddUsersToWarehouseDto
    {
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
}
