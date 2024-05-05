using Tuto_6.DTOs;
namespace Tuto_6.Services;

public interface IProductWarehouseService
{
    Task<bool> ProductExistsAsync(int productId);
    Task<bool> WarehouseExistsAsync(int warehouseId);
    Task<bool> IsAmountValidAsync(int amount);
    Task<bool> PurchaseOrderExistsAsync(int productId, int amount, DateTime requestCreatedAt);
    Task<bool> IsOrderCompletedAsync(int orderId);
    Task<bool> UpdateOrderFullfilledAtAsync(int orderId);
    Task<int?> InsertProductWarehouseRecordAsync(WarehouseOperation operation);
    Task<int?> RegisterProductInWarehouse(WarehouseOperation operation);

}