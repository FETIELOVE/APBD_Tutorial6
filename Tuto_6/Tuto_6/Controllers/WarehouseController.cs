using Tuto_6.DTOs;
using Tuto_6.Services;
using Microsoft.AspNetCore.Mvc;



namespace Tuto_6.Controllers;

[Route("api/[controller]")]
[ApiController]

public class WarehouseController: ControllerBase
{
    
     private readonly IProductWarehouseService _productWarehouseService;

        public WarehouseController(IProductWarehouseService productWarehouseService)
        {
            _productWarehouseService = productWarehouseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] WarehouseOperation request)
        {
            if (!(await _productWarehouseService.ProductExistsAsync(request.IdProductWarehouse)))
            {
                return NotFound("Product does not exist.");
            }

            if (!(await _productWarehouseService.WarehouseExistsAsync(request.IdWarehouse)))
            {
                return NotFound("Warehouse does not exist.");
            }

            if (!(await _productWarehouseService.PurchaseOrderExistsAsync(request.IdProductWarehouse, request.Amount, request.CreatedAt)))
            {
                return BadRequest("No matching purchase order found, or the order date is not valid.");
            }

            if (await _productWarehouseService.IsOrderCompletedAsync(request.Idorder))
            {
                return BadRequest("The order has already been completed");
            }

            if (!(await _productWarehouseService.UpdateOrderFullfilledAtAsync(request.Idorder)))
            {
                return StatusCode(400, "An error occurred while updating the order status.");
            }

            var insertedId = await _productWarehouseService.InsertProductWarehouseRecordAsync(new WarehouseOperation
            {
                IdWarehouse = request.IdWarehouse,
                IdProductWarehouse = request.IdProductWarehouse,
                Amount = request.Amount,
                CreatedAt = request.CreatedAt,
                Idorder = request.Idorder
            });

            if (!insertedId.HasValue)
            {
                return StatusCode(400, "An error occurred while inserting the product into the warehouse.");
            }

            return Ok(new { Message = "Product successfully added to the warehouse.", IdProductWarehouse = insertedId.Value });
        }
    }

    
    
    

    
