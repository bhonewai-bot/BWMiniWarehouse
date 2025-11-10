using MiniWarehouse.Database.AppDbContextModels;

namespace MiniWarehouse.ConsoleApp.EFCore;

public class InventoryTransactionEFCoreService
{
    private readonly AppDbContext _db;

    public InventoryTransactionEFCoreService(AppDbContext db)
    {
        _db = db;
    }

    public void StockIn(int itemId, int quantity)
    {
        var stock = _db.TblStocks.FirstOrDefault(x => x.ItemId == itemId);
        if (stock is null)
        {
            Console.WriteLine("Data not found.");
            return;
        }

        stock.Quantity += quantity;

        var inventoryTransaction = new TblInventoryTransaction()
        {
            ItemId = itemId,
            Type = EnumInventoryTransaction.In,
            Quantity = quantity,
        };
        _db.TblInventoryTransactions.Add(inventoryTransaction);
        
        int result = _db.SaveChanges();
        
        string message = result > 0 ? "Stock In Success." : "Stock In Failed.";
        Console.WriteLine(message);
    }

    public void StockOut(int itemId, int quantity)
    {
        var stock = _db.TblStocks.FirstOrDefault(x => x.ItemId == itemId);
        if (stock is null)
        {
            Console.WriteLine("Data not found.");
            return;
        }

        if (stock.Quantity <= 0 || stock.Quantity < quantity)
        {
            Console.WriteLine("Not enough stock.");
            return;
        }
        
        stock.Quantity -= quantity;
        
        var inventoryTransaction = new TblInventoryTransaction()
        {
            ItemId = itemId,
            Type = EnumInventoryTransaction.Out,
            Quantity = quantity,
        };
        _db.TblInventoryTransactions.Add(inventoryTransaction);
        
        int result = _db.SaveChanges();
        
        string message = result > 0 ? "Stock Out Success." : "Stock Out Failed.";
        Console.WriteLine(message);
    }
}