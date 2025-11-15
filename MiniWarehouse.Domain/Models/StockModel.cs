namespace MiniWarehouse.Domain.Models;

public class StockModel
{
    public int ItemId { get; set; }

    public int Quantity { get; set; }
    
    public int ReorderLevel { get; set; }
}