using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.MasterData.Application.Contracts.Suppliers
{
    public class CreateUpdateSupplierDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(50)]
        public string ContactPerson { get; set; }

        [StringLength(30)]
        public string Mobile { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(500)]
        public string Address { get; set; }
    }
}
