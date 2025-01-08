using System.ComponentModel.DataAnnotations;
using Lab2.Domain.Models;

namespace Lab2.API.Models
{
    public class InputOrder
    {
        // Product ID - required and must be a positive integer
        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductId { get; set; }

        // Quantity - required and must be a positive integer
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        public int Quantity { get; set; }
    }
}