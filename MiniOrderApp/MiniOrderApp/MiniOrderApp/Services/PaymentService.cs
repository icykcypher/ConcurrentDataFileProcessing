using MiniOrderApp.Models;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Shared;
using Microsoft.Data.SqlClient;

namespace MiniOrderApp.Services;

public class PaymentService(IConfiguration configuration) : IPaymentService
{
    public async Task<Result<Payment>> Add(Payment payment)
    {
        if (payment.OrderId <= 0)
            return Result<Payment>.Failure("InvalidOrderId", ErrorStatus.ValidationError);

        // Проверяем, что заказ существует
        const string checkOrderSql = "SELECT COUNT(1) FROM Orders WHERE Id = @OrderId";

        await using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        await using var checkCmd = new SqlCommand(checkOrderSql, conn);
        checkCmd.Parameters.AddWithValue("@OrderId", payment.OrderId);
        var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;

        if (!exists)
            return Result<Payment>.Failure("OrderNotFound", ErrorStatus.NotFound);

        const string sql = @"
        INSERT INTO Payments (OrderId, Amount, PaidAt, IsSuccessful)
        VALUES (@OrderId, @Amount, @PaidAt, @IsSuccessful);
        SELECT SCOPE_IDENTITY();
    ";

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@OrderId", payment.OrderId);
        cmd.Parameters.AddWithValue("@Amount", payment.PaidAmount);
        cmd.Parameters.AddWithValue("@PaidAt", payment.PaidAt);
        cmd.Parameters.AddWithValue("@IsSuccessful", payment.IsSuccessful);

        var idObj = await cmd.ExecuteScalarAsync();
        payment.Id = Convert.ToInt32(idObj);

        return Result<Payment>.Success(payment);
    }


    public async Task<Result<bool?>> MarkOrderPaid(int orderId)
    {
        const string sql = @"UPDATE Orders SET IsPaid = 1 WHERE Id = @OrderId";

        try
        {
            await using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0)
                return Result<bool?>.Failure("NotFound", ErrorStatus.NotFound);

            return Result<bool?>.Success(null);
        }
        catch (SqlException ex)
        {
            return Result<bool?>.Failure("DatabaseError", ErrorStatus.ServerError);
        }
        catch (Exception ex)
        {
            return Result<bool?>.Failure("ServerError", ErrorStatus.ServerError);
        }
    }
}
