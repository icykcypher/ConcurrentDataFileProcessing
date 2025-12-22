using MiniOrderApp.Models;
using Microsoft.Data.SqlClient;
using MiniOrderApp.Repositories.Interfaces;

namespace MiniOrderApp.Repositories;

public class CustomerRepository(IConfiguration cfg) : ICustomerRepository
{
        private readonly string _conn = cfg.GetConnectionString("DefaultConnection")
                                        ?? throw new ArgumentNullException("Default Connection string was not found");

        public async Task<IEnumerable<Customer>> GetAll()
        {
                var list = new List<Customer>();
                
                using var connection = new SqlConnection(_conn);
                using var cmd = new SqlCommand("SELECT Id, Name, Email, IsActive FROM Customers", connection);
                
                await connection.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                        list.Add(new Customer
                        {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                IsActive = reader.GetBoolean(3)
                        });
                }

                return list;
        }

        public async Task<Customer> GetById(int id)
        {
                using var connection = new SqlConnection(_conn);
                using var cmd = new SqlCommand("SELECT Id, Name, Email, IsActive FROM Customers WHERE Id = @id", connection);
                
                cmd.Parameters.AddWithValue("@id", id);
                
                await connection.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                        return new Customer
                        {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                IsActive = reader.GetBoolean(3)
                        };
                }

                return null!;
        }

        public async Task<Customer> Add(Customer customer)
        {
                using var connection = new SqlConnection(_conn);
                using var cmd = new SqlCommand(
                        "INSERT INTO Customers (Name, Email, IsActive) VALUES (@name, @email, @active); SELECT SCOPE_IDENTITY();",
                        connection);
                
                cmd.Parameters.AddWithValue("@name", customer.Name);
                cmd.Parameters.AddWithValue("@email", customer.Email);
                cmd.Parameters.AddWithValue("@active", customer.IsActive);
                
                await connection.OpenAsync();
                var id = await cmd.ExecuteScalarAsync();
                
                customer.Id = Convert.ToInt32(id);

                return customer;
        }

        public async Task<Customer> Update(Customer customer)
        {
                using var connection = new SqlConnection(_conn);
                using var cmd = new SqlCommand(
                        "UPDATE Customers SET Name=@name, Email=@email, IsActive=@active WHERE Id=@id", connection);
                
                cmd.Parameters.AddWithValue("@id", customer.Id);
                cmd.Parameters.AddWithValue("@name", customer.Name);
                cmd.Parameters.AddWithValue("@email", customer.Email);
                cmd.Parameters.AddWithValue("@active", customer.IsActive);
                
                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return customer;
        }

        public async Task Delete(int id)
        {
                using var connection = new SqlConnection(_conn);
                using var cmd = new SqlCommand("DELETE FROM Customers WHERE Id=@id", connection);
                
                cmd.Parameters.AddWithValue("@id", id);
                
                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
        }
}