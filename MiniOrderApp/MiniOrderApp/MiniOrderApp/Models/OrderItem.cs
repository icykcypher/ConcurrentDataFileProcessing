using System.ComponentModel.DataAnnotations;
using MiniOrderApp.Attributes;

namespace MiniOrderApp.Models;

public class OrderItem
{
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        [Required]
        [NotNegative]
        public int Quantity { get; set; }
        [Required]
        [NotNegative]
        public float UnitPrice { get; set; }
}