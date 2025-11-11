using MiniWarehouse.Database.AppDbContextModels;

namespace MiniWarehouse.Domain.Models;

public class InventoryTransactionModel
{
    public int ItemId { get; set; }

    public string Type { get; set; }

    public int Quantity { get; set; }
}