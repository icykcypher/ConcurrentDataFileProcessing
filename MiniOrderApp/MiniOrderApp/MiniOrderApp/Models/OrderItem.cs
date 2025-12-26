using System.ComponentModel.DataAnnotations;
using MiniOrderApp.Attributes;

namespace MiniOrderApp.Models;


public class OrderItem
{
        public int ProductId { get; private set; }
        [NotNegative] public int Quantity { get; private set; }
        [NotNegative] public float UnitPrice { get; private set; }

        private OrderItem() { } 

        public OrderItem(int productId, float unitPrice, int quantity)
        {
                if (productId <= 0) throw new ArgumentException("Invalid product id");
                if (unitPrice < 0) throw new ArgumentException("UnitPrice must be non-negative");
                if (quantity <= 0) throw new ArgumentException("Quantity must be positive");

                ProductId = productId;
                UnitPrice = unitPrice;
                Quantity = quantity;
        }
}