using MiniOrderApp.Models;
using MiniOrderApp.Repositories.Interfaces;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Services;

public class OrderService(
        IOrderRepository orderRepo,
        ICustomerRepository customerRepo,
        IProductRepository productRepo) : IOrderService
{
    public async Task<Result<int>> CreateOrder(int customerId, List<(int ProductId, int Quantity)> items)
    {
        var customer = await customerRepo.GetById(customerId);
        if (customer.Id == 0)
            return Result<int>.Failure("Customer not found", ErrorStatus.NotFound);

        if (!items.Any())
            return Result<int>.Failure("Order must contain items", ErrorStatus.ValidationError);

        var order = new Order(customerId);

        foreach (var item in items)
        {
            var product = await productRepo.GetById(item.ProductId);
            if (product.Id == 0)
                return Result<int>.Failure($"Product {item.ProductId} not found", ErrorStatus.NotFound);

            var r = order.AddItem(product.Id, product.Price, item.Quantity);
            if (!r.IsSuccess) return Result<int>.Failure(r.ErrorMessage!, r.Status!.Value);
        }

        var orderId = await orderRepo.CreateOrder(order);
        return Result<int>.Success(orderId);
    }

    public async Task<Result<bool?>> ConfirmOrder(int orderId)
    {
        var order = await orderRepo.GetById(orderId);
        if (order == null) return Result<bool?>.Failure("Order not found", ErrorStatus.NotFound);

        var r = order.Confirm();
        if (!r.IsSuccess) return r;

        await orderRepo.Update(order);
        return Result<bool?>.Success(null);
    }

    public async Task<Result<bool?>> CancelOrder(int orderId)
    {
        var order = await orderRepo.GetById(orderId);
        if (order == null) return Result<bool?>.Failure("Order not found", ErrorStatus.NotFound);

        var r = order.Cancel();
        if (!r.IsSuccess) return r;

        await orderRepo.Update(order);
        return Result<bool?>.Success(null);
    }
}
