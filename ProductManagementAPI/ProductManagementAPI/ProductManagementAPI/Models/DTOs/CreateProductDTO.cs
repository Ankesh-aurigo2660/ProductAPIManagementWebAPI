using System.ComponentModel.DataAnnotations;

namespace ProductManagementAPI.Models.DTOs
{
    public class CreateProductDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative")]
        public int StockAvailable { get; set; }

        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
    }
}
