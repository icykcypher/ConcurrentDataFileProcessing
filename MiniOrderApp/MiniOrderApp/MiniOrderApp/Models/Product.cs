using System.ComponentModel.DataAnnotations;
using MiniOrderApp.Attributes;

namespace MiniOrderApp.Models;


public class Product
{
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; } =  string.Empty;
        [Required]
        [NotNegative]
        public float Price { get; set; }
        public ProductCategory Category { get; set; }
}