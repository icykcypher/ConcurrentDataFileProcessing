using System.ComponentModel.DataAnnotations;
using MiniOrderApp.Models;
using MiniOrderApp.Repositories.Interfaces;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Services;

public class CustomerService(ICustomerRepository repo) : ICustomerService
{
    public async Task<IEnumerable<Customer>> GetAll() => await repo.GetAll();

    public async Task<Result<Customer>> GetById(int id)
    {
        var customer = await repo.GetById(id);
        if (IsEmptyCustomer(customer))
            return Result<Customer>.Failure($"Customer with id {id} not found", ErrorStatus.NotFound);

        return Result<Customer>.Success(customer);
    }

    public async Task<Result<Customer>> Add(Customer customer)
    {
        var errors = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(customer, new ValidationContext(customer), errors, true);
        if (!isValid)
            return Result<Customer>.ValidationFailure(errors);

        var exists = await repo.Exists(customer);
        if (exists)
            return Result<Customer>.Failure("Customer with the same email already exists", ErrorStatus.Conflict);

        var added = await repo.Add(customer);
        return Result<Customer>.Success(added);
    }

    public async Task<Result<Customer>> Update(Customer customer)
    {
        var existing = await repo.GetById(customer.Id);
        if (IsEmptyCustomer(existing))
            return Result<Customer>.Failure($"Customer with id {customer.Id} not found", ErrorStatus.NotFound);

        var errors = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(customer, new ValidationContext(customer), errors, true);
        if (!isValid)
            return Result<Customer>.ValidationFailure(errors);

        var allCustomers = await repo.GetAll();
        var duplicate = allCustomers.Any(c =>
            c.Id != customer.Id &&
            string.Equals(c.Email, customer.Email, StringComparison.OrdinalIgnoreCase)
        );
        if (duplicate)
            return Result<Customer>.Failure("Another customer with the same email already exists", ErrorStatus.Conflict);

        await repo.Update(customer);
        return Result<Customer>.Success(customer);
    }

    public async Task<Result<bool>> Delete(int id)
    {
        var existing = await repo.GetById(id);
        if (IsEmptyCustomer(existing))
            return Result<bool>.Failure($"Customer with id {id} not found", ErrorStatus.NotFound);

        await repo.Delete(id);
        return Result<bool>.Success(true);
    }

    private bool IsEmptyCustomer(Customer c) =>
        c.Id == 0 &&
        string.IsNullOrWhiteSpace(c.Name) &&
        string.IsNullOrWhiteSpace(c.Email) &&
        c.IsActive == false;
}
