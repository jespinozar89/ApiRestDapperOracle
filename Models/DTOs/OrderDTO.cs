using System.ComponentModel.DataAnnotations;

namespace MyApiRestDapperOracle.Models.DTOs
{
    public class OrderDTO
    {
        [Required]
        public int OrderId { get; set; } //PK
        [Required]
        public int CustomerId { get; set; } //FK
        [Required]
        [RegularExpression(@"^(Pending|Shipped|Delivered|Cancelled)$",
            ErrorMessage = "Order status must be one of the following: Pending, Shipped, Delivered, Cancelled")]
        public string OrderStatus { get; set; }
    }
}