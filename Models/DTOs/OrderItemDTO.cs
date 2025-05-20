using System.ComponentModel.DataAnnotations;

namespace MyApiRestDapperOracle.Models.DTOs
{
    public class OrderItemDTO
    {
        [Required]
        public int LineItemId { get; set; } //PK
        [Required]
        public int OrderId { get; set; } //FK
        [Required]
        public int ProductId { get; set; } //FK
        [Required]
        [Range(1000, int.MaxValue, ErrorMessage = "Quantity must be greater than 999")]
        public int UnitPrice { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}