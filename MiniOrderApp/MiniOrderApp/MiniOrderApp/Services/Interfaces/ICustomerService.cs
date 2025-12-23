using MiniOrderApp.Models;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Services.Interfaces;

public interface ICustomerService
{
        Task<IEnumerable<Customer>> GetAll();
        Task<Result<Customer>> GetById(int id);
        Task<Result<Customer>> Add(Customer customer);
        Task<Result<Customer>> Update(Customer customer);
        Task<Result<bool>> Delete(int id);
}