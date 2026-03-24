using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.Identity.OrganizationUnits.Dtos
{
    public class UpdateOrganizationUnitDto
    {
        [Required]
        public string DisplayName { get; set; }
    }
}

