using MiniOrderApp.Models;

namespace MiniOrderApp.Dtos;

public class ProductCreateDto
{
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public ProductCategory Category { get; set; }
}