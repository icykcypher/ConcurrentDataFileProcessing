using Microsoft.AspNetCore.Mvc;
using MiniOrderApp.Dtos;
using MiniOrderApp.Models;
using MiniOrderApp.Services.Interfaces;

namespace MiniOrderApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IPaymentService service) : ControllerBase
{
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDto dto)
        {
                var payment = new Payment
                {
                        OrderId = dto.OrderId,
                        PaidAmount = dto.PaidAmount,
                        PaidAt = DateTime.UtcNow,
                        IsSuccessful = true 
                };

                var result = await service.Add(payment);
                if (!result.IsSuccess)
                        return BadRequest(new { message = result.ErrorMessage });

                await service.MarkOrderPaid(dto.OrderId);

                return Ok(new { data = result.Value });
        }
}