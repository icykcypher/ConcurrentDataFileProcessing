using MiniOrderApp.Models;
using Microsoft.Data.SqlClient;
using MiniOrderApp.Repositories.Interfaces;

namespace MiniOrderApp.Repositories;

public class ProductRepository(IConfiguration cfg) : IProductRepository
{
    private readonly string _conn = cfg.GetConnectionString("DefaultConnection")
                                    ?? throw new ArgumentNullException("Default Connection string was not found");

    public async Task<IEnumerable<Product>> GetAll()
    {
        var list = new List<Product>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "SELECT Id, Name, Price, Category FROM Products", con);
        await con.OpenAsync();
        var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = (float)reader.GetDouble(2), 
                Category = (ProductCategory)reader.GetInt32(3)
            });
        }
        return list;
    }

    public async Task<Product> GetById(int id)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "SELECT Id, Name, Price, Category FROM Products WHERE Id = @id", con);
        cmd.Parameters.AddWithValue("@id", id);
        await con.OpenAsync();
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = (float)reader.GetDouble(2),
                Category = (ProductCategory)reader.GetInt32(3)
            };
        }
        return null!; 
    }

    public async Task<Product> Add(Product product)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "INSERT INTO Products (Name, Price, Category) VALUES (@name, @price, @category); SELECT SCOPE_IDENTITY();", con);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@category", (int)product.Category);
        await con.OpenAsync();
        var id = await cmd.ExecuteScalarAsync();
        product.Id = Convert.ToInt32(id);
        
        return product;
    }

    public async Task<Product> Update(Product product)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "UPDATE Products SET Name=@name, Price=@price, Category=@category WHERE Id=@id", con);
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@category", (int)product.Category);
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        return product;
    }

    public async Task Delete(int id)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "DELETE FROM Products WHERE Id=@id", con);
        cmd.Parameters.AddWithValue("@id", id);
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
