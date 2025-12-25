using MiniOrderApp.Repositories.Interfaces;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Services;

public class OrderService(
        IOrderRepository orderRepo,
        ICustomerRepository customerRepo,
        IProductRepository productRepo)
        : IOrderService
{
        public async Task<Result<int>> CreateOrder(
                int customerId,
                List<(int ProductId, int Quantity)> items)
        {
                var customer = await customerRepo.GetById(customerId);
                if (customer.Id == 0)
                        return Result<int>.Failure("Customer not found", ErrorStatus.NotFound);

                if (!items.Any())
                        return Result<int>.Failure("Order must contain items", ErrorStatus.ValidationError);

                var products = new List<(Models.Product Product, int Quantity)>();

                foreach (var item in items)
                {
                        var product = await productRepo.GetById(item.ProductId);
                        if (product.Id == 0)
                                return Result<int>.Failure($"Product {item.ProductId} not found", ErrorStatus.NotFound);

                        products.Add((product, item.Quantity));
                }

                var orderId = await orderRepo.CreateOrder(customerId, products);
                return Result<int>.Success(orderId);
        }
}