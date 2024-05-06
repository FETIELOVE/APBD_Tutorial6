using Tuto_6.DTOs;
using System.Data;
using Microsoft.Data.SqlClient;
namespace Tuto_6.Services;

public class ProductWarehouseService : IProductWarehouseService
{

     private readonly IConfiguration _configuration;
   
           public ProductWarehouseService(IConfiguration configuration)
           {
               _configuration = configuration;
           }
   
           public async Task<bool> ProductExistsAsync(int productId)
           {
               using var connection = await GetSqlConnectionAsync();
               var query = "SELECT COUNT(*) FROM Product WHERE Id = @ProductId";
               using var command = new SqlCommand(query, connection);
               command.Parameters.AddWithValue("@ProductId", productId);
               var result = await command.ExecuteScalarAsync();
               int count = Convert.ToInt32(result);
               return count > 0;
           }
   
           public async Task<bool> WarehouseExistsAsync(int warehouseId)
           {
               using var connection = await GetSqlConnectionAsync();
               var query = "SELECT COUNT(*) FROM Warehouse WHERE Id = @WarehouseId";
               using var command = new SqlCommand(query, connection);
               command.Parameters.AddWithValue("@WarehouseId", warehouseId);
               var result = await command.ExecuteScalarAsync();
               return result != null && (int)result > 0;
           }
   
           public Task<bool> IsAmountValidAsync(int amount)
           {
               return Task.Run(() => amount > 0);
           }
           public async Task<bool> PurchaseOrderExistsAsync(int productId, int amount, DateTime requestCreatedAt)
           {
               using var connection = await GetSqlConnectionAsync();
               var query = "SELECT COUNT(*) FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @RequestCreatedAt";
               using var command = new SqlCommand(query, connection);
               command.Parameters.AddWithValue("@ProductId", productId);
               command.Parameters.AddWithValue("@Amount", amount);
               command.Parameters.AddWithValue("@RequestCreatedAt", requestCreatedAt);
               var result = await command.ExecuteScalarAsync();
               return result != null && (int)result > 0;
               }
           public async Task<bool> IsOrderCompletedAsync(int idOrder)
           {
               using var connection = await GetSqlConnectionAsync();
               var query = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
               using var command = new SqlCommand(query, connection);
               command.Parameters.AddWithValue("@IdOrder", idOrder);
               var result = await command.ExecuteScalarAsync();
               return result != null && (int)result > 0;
           }
   
          public async Task<bool> UpdateOrderFullfilledAtAsync(int idOrder)
          {
              try
              {
                  using var connection = await GetSqlConnectionAsync();
                  var query = "UPDATE [Order] SET FulfilledAt = @CurrentDateTime WHERE Id = @IdOrder";
                  using var command = new SqlCommand(query, connection);
                  command.Parameters.AddWithValue("@CurrentDateTime", DateTime.Now);
                  command.Parameters.AddWithValue("@IdOrder", idOrder);
                  int rowsAffected = await command.ExecuteNonQueryAsync();
                  return rowsAffected > 0;
              }
              catch (Exception ex)
              {
                  Console.WriteLine($"An error occurred while updating order fulfillment: {ex.Message}");
                  return false;
              }
          }
           public async Task<int?> InsertProductWarehouseRecordAsync(WarehouseOperation operation)
           {
               
               decimal productPrice = await GetProductPrice();
               using var connection = await GetSqlConnectionAsync();
               var query = "INSERT INTO Product_Warehouse (IdOrder, IdProduct, IdWarehouse, Amount, Price, CreatedAt) " +
                           "VALUES (@IdOrder, @IdProduct, @IdWarehouse, @Amount, @Price, @CreatedAt);" +
                           "SELECT SCOPE_IDENTITY();";
               using var command = new SqlCommand(query, connection);
               command.Parameters.AddWithValue("@IdOrder", operation.Idorder);
               command.Parameters.AddWithValue("@IdProduct", operation.IdProductWarehouse);
               command.Parameters.AddWithValue("@IdWarehouse", operation.IdWarehouse);
               command.Parameters.AddWithValue("@Amount", operation.Amount);
               command.Parameters.AddWithValue("@Price", productPrice * operation.Amount);
               command.Parameters.AddWithValue("@CreatedAt", operation.CreatedAt);
               var result = await command.ExecuteScalarAsync();
              return result != DBNull.Value ? Convert.ToInt32(result) : null;
           }
   
           private Task<decimal> GetProductPrice()
           {
               throw new NotImplementedException();
           }
    
    public async Task<int?> RegisterProductInWarehouse(WarehouseOperation operation)
    {
        
        if (!(await ProductExistsAsync(operation.IdProductWarehouse)))
            return null;
    
        
        if (!(await WarehouseExistsAsync(operation.IdWarehouse)))
            return null;
    
       
        if (!(await IsAmountValidAsync(operation.Amount)))
            return null;
    
        
        if (!(await PurchaseOrderExistsAsync(operation.IdProductWarehouse, operation.Amount, operation.CreatedAt)))
            return null;
    
        
        if (await IsOrderCompletedAsync(operation.Idorder))
            return null;
    
        
        if (!(await UpdateOrderFullfilledAtAsync(operation.Idorder)))
            return null;
    
       
        return await InsertProductWarehouseRecordAsync(operation);
    }
   
    public async Task<int?> ExecuteStoredProcedureAsync(WarehouseOperation operation)
    {
        try
        {
            using var connection = await GetSqlConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "InsertProductWarehouse"; 

           
            command.Parameters.AddWithValue("@IdOrder", operation.Idorder);
            command.Parameters.AddWithValue("@IdProduct", operation.IdProductWarehouse);
            command.Parameters.AddWithValue("@IdWarehouse", operation.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", operation.Amount);
            command.Parameters.AddWithValue("@CreatedAt", operation.CreatedAt);
            var result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : null;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"An error occurred while executing the stored procedure: {ex.Message}");
            return null; 
        }
    }

    
           private async Task<SqlConnection> GetSqlConnectionAsync()
           {
               var connectionString = _configuration.GetConnectionString("Data Source=db-mssql16;Initial Catalog=2019SBD;Integrated Security=True");
               var connection = new SqlConnection(connectionString);
               await connection.OpenAsync();
               return connection;
           }
       }
   