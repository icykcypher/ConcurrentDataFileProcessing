using System.ComponentModel.DataAnnotations;
using MiniOrderApp.Attributes;

namespace MiniOrderApp.Models;

public class Payment
{
        public int Id { get; set; }
        public int OrderId { get; set; }
        [Required]
        [NotNegative]
        public float PaidAmount { get; set; }
        [Required]
        public DateTime PaidAt { get; set; }
        public bool IsSuccessful { get; set; }
}