using Tuto_6.DTOs;
using Tuto_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace Tuto_6.Controllers;

[Route("api/[controller]")]
[ApiController]


public class WarehouseController2 : ControllerBase

{
    private readonly IProductWarehouseService _productWarehouseService;

    public WarehouseController2(IProductWarehouseService productWarehouseService)
    {
        _productWarehouseService = productWarehouseService;
    }

    [HttpPost("newendpoint")]
    public async Task<IActionResult> NewEndpoint([FromBody] WarehouseOperation request)
    {
        try
        {
           
            var insertedId = await _productWarehouseService.ExecuteStoredProcedureAsync(request);

            if (!insertedId.HasValue)
            {
                return StatusCode(400, "An error occurred while executing the stored procedure.");
            }

            return Ok(new { Message = "Product successfully added to the warehouse.", IdProductWarehouse = insertedId.Value });
        }
        catch
        {
           
            Console.WriteLine("An error occurred while executing the stored procedure.");
            return StatusCode(400, "An error occurred while executing the stored procedure.");
        }
    }
}