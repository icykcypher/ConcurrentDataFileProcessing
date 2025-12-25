using MiniOrderApp.Shared;

namespace MiniOrderApp.Services.Interfaces;

public interface IOrderService
{
        Task<Result<int>> CreateOrder(int customerId, List<(int ProductId, int Quantity)> items);
}