using System.ComponentModel.DataAnnotations;

namespace MyApiRestDapperOracle.Models.DTOs
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
            ErrorMessage = "Please enter a valid email address (e.g., user@example.com)")]
        public string? EmailAddress { get; set; }
        [Required]
        [MaxLength(255)]
        public string? FullName { get; set; }
    }
}