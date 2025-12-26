using MiniOrderApp.Models;

namespace MiniOrderApp.Repositories.Interfaces;

public interface IOrderRepository
{
        Task<int> CreateOrder(Order order);
        Task<Order?> GetById(int id);
        Task<IEnumerable<Order>> GetAll();
        Task Update(Order order);
        Task Delete(int id);
}