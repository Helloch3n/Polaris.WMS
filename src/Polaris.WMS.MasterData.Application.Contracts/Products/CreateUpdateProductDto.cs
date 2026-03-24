using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.MasterData.Application.Contracts.Products
{
    public class CreateUpdateProductDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Unit { get; set; }

        [MaxLength(20)]
        public string AuxUnit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ConversionRate { get; set; }

        public bool IsBatchManagementEnabled { get; set; }

        public int? ShelfLifeDays { get; set; }
    }
}
