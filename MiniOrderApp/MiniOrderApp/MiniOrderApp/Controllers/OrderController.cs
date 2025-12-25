using Microsoft.AspNetCore.Mvc;
using MiniOrderApp.Dtos;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService service) : ControllerBase
{
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
                var items = dto.Items
                        .Select(i => (i.ProductId, i.Quantity))
                        .ToList();

                var result = await service.CreateOrder(dto.CustomerId, items);
                return FromResult(result);
        }

        private IActionResult FromResult<T>(Result<T> result)
        {
                if (result.IsSuccess) return Ok(new { data = result.Value });

                return result.Status switch
                {
                        ErrorStatus.ValidationError => BadRequest(new { message = result.ErrorMessage }),
                        ErrorStatus.NotFound => NotFound(new { message = result.ErrorMessage }),
                        ErrorStatus.Conflict => Conflict(new { message = result.ErrorMessage }),
                        ErrorStatus.ServerError => StatusCode(500, new { message = result.ErrorMessage }),
                        _ => BadRequest(new { message = result.ErrorMessage })
                };
        }
}
