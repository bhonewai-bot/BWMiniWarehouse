namespace MiniWarehouse.Domain.Models;

public class ItemModel
{
    public int ItemId { get; set; }

    public string Sku { get; set; } = null!;

    public string ItemName { get; set; } = null!;
}