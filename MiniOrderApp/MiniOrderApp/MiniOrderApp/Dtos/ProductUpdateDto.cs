using MiniOrderApp.Models;

namespace MiniOrderApp.Dtos;

public class ProductUpdateDto
{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public ProductCategory Category { get; set; }
}