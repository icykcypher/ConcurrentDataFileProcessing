using MiniOrderApp.Dtos;
using Microsoft.AspNetCore.Mvc;
using MiniOrderApp.Models;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await service.GetAll();
        return Ok(new { data = customers });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetById(id);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        var result = await service.Add(customer);
        return FromResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto)
    {
        var customer = new Customer
        {
            Id = id,
            Name = dto.Name,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        var result = await service.Update(customer);
        return FromResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await service.Delete(id);
        return FromResult(result);
    }

    private IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(new { data = result.Value });

        return result.Status switch
        {
            ErrorStatus.ValidationError => BadRequest(new { message = result.ErrorMessage }),
            ErrorStatus.Conflict => Conflict(new { message = result.ErrorMessage }),
            ErrorStatus.NotFound => NotFound(new { message = result.ErrorMessage }),
            ErrorStatus.Unauthorized => Unauthorized(new { message = result.ErrorMessage }),
            ErrorStatus.Forbidden => Forbid(),
            ErrorStatus.ServerError => StatusCode(500, new { message = result.ErrorMessage }),
            _ => BadRequest(new { message = result.ErrorMessage })
        };
    }
}
