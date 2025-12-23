using MiniOrderApp.Models;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Services.Interfaces;


public interface IProductService
{
        Task<IEnumerable<Product>> GetAll();
        Task<Result<Product>> GetById(int id);
        Task<Result<Product>> Add(Product product);
        Task<Result<Product>> Update(Product product);
        Task<Result<bool>> Delete(int id);
}