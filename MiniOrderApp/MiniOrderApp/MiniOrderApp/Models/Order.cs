using System.ComponentModel.DataAnnotations;
using MiniOrderApp.Attributes;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Models;

public class Order
{
        public int Id { get; private set; }
        public int CustomerId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public List<OrderItem> Items { get; private set; } = [];
        public float TotalPrice => Items.Sum(i => i.Quantity * i.UnitPrice);

        private Order() { } 

        public Order(int customerId)
        {
                if (customerId <= 0) throw new ArgumentException("Invalid customer id");
                CustomerId = customerId;
                OrderDate = DateTime.UtcNow;
        }

        public Result<bool?> AddItem(int productId, float unitPrice, int quantity)
        {
                if (Status != OrderStatus.Pending)
                        return Result<bool?>.Failure("Cannot modify order after confirmation", ErrorStatus.ValidationError);

                if (quantity <= 0) return Result<bool?>.Failure("Quantity must be positive", ErrorStatus.ValidationError);
                if (unitPrice < 0) return Result<bool?>.Failure("UnitPrice must be non-negative", ErrorStatus.ValidationError);

                Items.Add(new OrderItem(productId, unitPrice, quantity));
                return Result<bool?>.Success(null);
        }

        public Result<bool?> Confirm()
        {
                if (!Items.Any()) return  Result<bool?>.Failure("Cannot confirm order without items", ErrorStatus.ValidationError);
                Status = OrderStatus.Confirmed;
                return  Result<bool?>.Success(null);
        }

        public Result<bool?> Cancel()
        {
                if (Status == OrderStatus.Confirmed) return Result<bool?>.Failure("Cannot cancel confirmed order", ErrorStatus.ValidationError);
                Status = OrderStatus.Cancelled;
                return Result<bool?>.Success(null);
        }
        
        public static Order Restore(
                int id,
                int customerId,
                DateTime orderDate,
                OrderStatus status,
                IEnumerable<OrderItem> items)
        {
                var order = new Order(customerId)
                {
                        Id = id,
                        OrderDate = orderDate,
                        Status = status,
                        Items = items.ToList()
                };

                return order;
        }
}