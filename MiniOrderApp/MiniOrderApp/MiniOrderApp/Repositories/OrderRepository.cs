using MiniOrderApp.Models;
using MiniOrderApp.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace MiniOrderApp.Repositories;

public class OrderRepository(IConfiguration cfg) : IOrderRepository
{
        private readonly string _conn =
                cfg.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string not found");

        public async Task<int> CreateOrder(Order order)
        {
                using var con = new SqlConnection(_conn);
                await con.OpenAsync();
                using var tx = con.BeginTransaction();

                try
                {
                        var orderCmd = new SqlCommand(
                                """
                                INSERT INTO Orders (CustomerId, OrderDate, Status)
                                OUTPUT INSERTED.Id
                                VALUES (@customerId, @orderDate, @status)
                                """, con, tx);

                        orderCmd.Parameters.AddWithValue("@customerId", order.CustomerId);
                        orderCmd.Parameters.AddWithValue("@orderDate", order.OrderDate);
                        orderCmd.Parameters.AddWithValue("@status", order.Status.ToString());

                        var orderId = (int)await orderCmd.ExecuteScalarAsync();

                        foreach (var item in order.Items)
                        {
                                var itemCmd = new SqlCommand(
                                        """
                                        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                                        VALUES (@orderId, @productId, @qty, @price)
                                        """, con, tx);

                                itemCmd.Parameters.AddWithValue("@orderId", orderId);
                                itemCmd.Parameters.AddWithValue("@productId", item.ProductId);
                                itemCmd.Parameters.AddWithValue("@qty", item.Quantity);
                                itemCmd.Parameters.AddWithValue("@price", item.UnitPrice);

                                await itemCmd.ExecuteNonQueryAsync();
                        }

                        tx.Commit();
                        return orderId;
                }
                catch
                {
                        tx.Rollback();
                        throw;
                }
        }

        public async Task<Order?> GetById(int id)
        {
                using var con = new SqlConnection(_conn);
                await con.OpenAsync();

                int customerId;
                DateTime orderDate;
                OrderStatus status;

                var orderCmd = new SqlCommand(
                        """
                        SELECT CustomerId, OrderDate, Status
                        FROM Orders
                        WHERE Id = @id
                        """, con);

                orderCmd.Parameters.AddWithValue("@id", id);

                using (var reader = await orderCmd.ExecuteReaderAsync())
                {
                        if (!await reader.ReadAsync())
                                return null;

                        customerId = reader.GetInt32(0);
                        orderDate = reader.GetDateTime(1);
                        status = Enum.Parse<OrderStatus>(reader.GetString(2));
                }

                var items = new List<OrderItem>();

                var itemsCmd = new SqlCommand(
                        """
                        SELECT ProductId, UnitPrice, Quantity
                        FROM OrderItems
                        WHERE OrderId = @orderId
                        """, con);

                itemsCmd.Parameters.AddWithValue("@orderId", id);

                using var itemsReader = await itemsCmd.ExecuteReaderAsync();
                while (await itemsReader.ReadAsync())
                {
                        items.Add(new OrderItem(
                                itemsReader.GetInt32(0),
                                itemsReader.GetFloat(1),
                                itemsReader.GetInt32(2)
                        ));
                }

                return Order.Restore(id, customerId, orderDate, status, items);
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
                var orders = new Dictionary<int, (
                        int CustomerId,
                        DateTime OrderDate,
                        OrderStatus Status,
                        List<OrderItem> Items
                        )>();

                using var con = new SqlConnection(_conn);
                await con.OpenAsync();

                var cmd = new SqlCommand(
                        """
                        SELECT o.Id, o.CustomerId, o.OrderDate, o.Status, i.ProductId, i.Quantity, i.UnitPrice
                        FROM Orders o
                        LEFT JOIN OrderItems i ON o.Id = i.OrderId
                        ORDER BY o.Id
                        """, con);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                        var orderId = reader.GetInt32(0);

                        if (!orders.TryGetValue(orderId, out var entry))
                        {
                                entry = (
                                        reader.GetInt32(1),
                                        reader.GetDateTime(2),
                                        Enum.Parse<OrderStatus>(reader.GetString(3)),
                                        new List<OrderItem>()
                                );

                                orders.Add(orderId, entry);
                        }

                        if (!reader.IsDBNull(4))
                        {
                                entry.Items.Add(new OrderItem(
                                        reader.GetInt32(4),
                                        reader.GetFloat(6),
                                        reader.GetInt32(5)
                                ));
                        }

                        orders[orderId] = entry;
                }

                return orders.Select(o =>
                        Order.Restore(
                                o.Key,
                                o.Value.CustomerId,
                                o.Value.OrderDate,
                                o.Value.Status,
                                o.Value.Items
                        ));
        }


        public async Task Update(Order order)
        {
                using var con = new SqlConnection(_conn);
                await con.OpenAsync();
                using var tx = con.BeginTransaction();

                try
                {
                        var updateCmd = new SqlCommand(
                                """
                                UPDATE Orders
                                SET Status = @status
                                WHERE Id = @id
                                """, con, tx);

                        updateCmd.Parameters.AddWithValue("@id", order.Id);
                        updateCmd.Parameters.AddWithValue("@status", order.Status.ToString());

                        await updateCmd.ExecuteNonQueryAsync();

                        var deleteItems = new SqlCommand(
                                "DELETE FROM OrderItems WHERE OrderId = @orderId", con, tx);

                        deleteItems.Parameters.AddWithValue("@orderId", order.Id);
                        await deleteItems.ExecuteNonQueryAsync();

                        foreach (var item in order.Items)
                        {
                                var insertItem = new SqlCommand(
                                        """
                                        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                                        VALUES (@orderId, @productId, @qty, @price)
                                        """, con, tx);

                                insertItem.Parameters.AddWithValue("@orderId", order.Id);
                                insertItem.Parameters.AddWithValue("@productId", item.ProductId);
                                insertItem.Parameters.AddWithValue("@qty", item.Quantity);
                                insertItem.Parameters.AddWithValue("@price", item.UnitPrice);

                                await insertItem.ExecuteNonQueryAsync();
                        }

                        tx.Commit();
                }
                catch
                {
                        tx.Rollback();
                        throw;
                }
        }

        public async Task Delete(int id)
        {
                using var con = new SqlConnection(_conn);
                await con.OpenAsync();
                using var tx = con.BeginTransaction();

                try
                {
                        var deleteItems = new SqlCommand(
                                "DELETE FROM OrderItems WHERE OrderId = @id", con, tx);
                        deleteItems.Parameters.AddWithValue("@id", id);
                        await deleteItems.ExecuteNonQueryAsync();

                        var deleteOrder = new SqlCommand(
                                "DELETE FROM Orders WHERE Id = @id", con, tx);
                        deleteOrder.Parameters.AddWithValue("@id", id);
                        await deleteOrder.ExecuteNonQueryAsync();

                        tx.Commit();
                }
                catch
                {
                        tx.Rollback();
                        throw;
                }
        }
}