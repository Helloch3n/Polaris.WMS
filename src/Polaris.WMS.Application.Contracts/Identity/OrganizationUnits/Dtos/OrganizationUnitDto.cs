using System;

namespace Polaris.WMS.Identity.OrganizationUnits.Dtos
{
    public class OrganizationUnitDto
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
    }
}

