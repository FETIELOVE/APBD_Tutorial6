using System.ComponentModel.DataAnnotations;
namespace Tuto_6.DTOs;

public class WarehouseOperation
{
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    public int IdProductWarehouse { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public int Idorder { get; set; }
    
    
}