using System.ComponentModel.DataAnnotations;
using MiniOrderApp.Attributes;

namespace MiniOrderApp.Models;

public class Order
{
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        [Required]
        [NotNegative]
        public float TotalPrice { get; set; }
}