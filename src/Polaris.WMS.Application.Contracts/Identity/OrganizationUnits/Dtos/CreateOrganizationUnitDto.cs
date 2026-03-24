using System;
using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.Identity.OrganizationUnits.Dtos
{
    public class CreateOrganizationUnitDto
    {
        [Required]
        public string DisplayName { get; set; }

        public Guid? ParentId { get; set; }
    }
}

