using MiniOrderApp.Models;

namespace MiniOrderApp.Repositories.Interfaces;

public interface IProductRepository
{
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetById(int id);
        Task<Product> Add(Product product);
        Task<Product> Update(Product product);
        Task Delete(int id);
        Task<bool> Exists(Product product);
}