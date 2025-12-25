using MiniOrderApp.Models;
using Microsoft.Data.SqlClient;
using MiniOrderApp.Repositories.Interfaces;

namespace MiniOrderApp.Repositories;

public class OrderRepository(IConfiguration cfg) : IOrderRepository
{
    private readonly string _conn =
        cfg.GetConnectionString("DefaultConnection")
        ?? throw new ArgumentNullException("Connection string not found");

    public async Task<int> CreateOrder(
        int customerId,
        IEnumerable<(Product Product, int Quantity)> items)
    {
        using var con = new SqlConnection(_conn);
        await con.OpenAsync();

        using var tx = con.BeginTransaction();

        try
        {
            var orderCmd = new SqlCommand(
                """
                INSERT INTO Orders (CustomerId, Status)
                OUTPUT INSERTED.Id
                VALUES (@customerId, 'Created')
                """, con, tx);

            orderCmd.Parameters.AddWithValue("@customerId", customerId);
            int orderId = (int)await orderCmd.ExecuteScalarAsync();

            foreach (var (product, qty) in items)
            {
                var itemCmd = new SqlCommand(
                    """
                    INSERT INTO OrderItems
                    (OrderId, ProductId, Quantity, PriceAtOrder)
                    VALUES (@orderId, @productId, @qty, @price)
                    """, con, tx);

                itemCmd.Parameters.AddWithValue("@orderId", orderId);
                itemCmd.Parameters.AddWithValue("@productId", product.Id);
                itemCmd.Parameters.AddWithValue("@qty", qty);
                itemCmd.Parameters.AddWithValue("@price", product.Price);

                await itemCmd.ExecuteNonQueryAsync();
            }

            var total = items.Sum(i => i.Product.Price * i.Quantity);

            var paymentCmd = new SqlCommand(
                """
                INSERT INTO Payments (OrderId, Amount, IsSuccessful)
                VALUES (@orderId, @amount, 1)
                """, con, tx);

            paymentCmd.Parameters.AddWithValue("@orderId", orderId);
            paymentCmd.Parameters.AddWithValue("@amount", total);

            await paymentCmd.ExecuteNonQueryAsync();

            tx.Commit();
            return orderId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
