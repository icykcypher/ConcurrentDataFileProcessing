using MiniOrderApp.Dtos;
using MiniOrderApp.Shared;
using MiniOrderApp.Models;
using Microsoft.AspNetCore.Mvc;
using MiniOrderApp.Services.Interfaces;

namespace MiniOrderApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await service.GetAll();
        return Ok(new { data = products });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetById(id);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Category = dto.Category
        };

        var result = await service.Add(product);
        return FromResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        var product = new Product
        {
            Id = id,
            Name = dto.Name,
            Price = dto.Price,
            Category = dto.Category
        };

        var result = await service.Update(product);
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
