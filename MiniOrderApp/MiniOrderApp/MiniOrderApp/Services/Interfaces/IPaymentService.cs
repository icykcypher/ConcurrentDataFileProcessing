using MiniOrderApp.Models;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Services.Interfaces;

public interface IPaymentService
{
        Task<Result<Payment>> Add(Payment payment);
        Task<Result<bool?>> MarkOrderPaid(int orderId);
}