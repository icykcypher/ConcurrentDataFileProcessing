using MiniOrderApp.Models;

namespace MiniOrderApp.Repositories.Interfaces;

public interface IOrderRepository
{
        Task<int> CreateOrder(int customerId, IEnumerable<(Product Product, int Quantity)> items);
}