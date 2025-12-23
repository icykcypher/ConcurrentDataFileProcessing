using MiniOrderApp.Models;

namespace MiniOrderApp.Repositories.Interfaces;

public interface ICustomerRepository
{
        Task<IEnumerable<Customer>> GetAll();
        Task<Customer> GetById(int id);
        Task<Customer> Add(Customer c);
        Task<Customer> Update(Customer customer);
        Task Delete(int id);
        Task<bool> Exists(Customer customer);
}