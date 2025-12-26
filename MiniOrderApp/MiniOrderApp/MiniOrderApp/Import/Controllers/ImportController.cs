using Microsoft.AspNetCore.Mvc;
using MiniOrderApp.Import.Services;

namespace MiniOrderApp.Import.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController(IImportService service) : ControllerBase
{
        [HttpPost("products")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
                using var stream = file.OpenReadStream();
                var result = await service.ImportProducts(stream);
                return Ok(new { imported = result.Value });
        }

        [HttpPost("customers")]
        public async Task<IActionResult> ImportCustomers(IFormFile file)
        {
                using var stream = file.OpenReadStream();
                var result = await service.ImportCustomers(stream);
                return Ok(new { imported = result.Value });
        }
}