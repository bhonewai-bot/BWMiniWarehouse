using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.EFCore;

public class InventoryTransactionEFCoreService
{
    private readonly AppDbContext _db;

    public InventoryTransactionEFCoreService()
    {
        _db = new AppDbContext();
    }

    public void StockIn(int itemId, int quantity)
    {
        var stock = _db.TblStocks.FirstOrDefault(x => x.ItemId == itemId);
        if (stock is null)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }

        _db.TblInventoryTransactions.Add(new TblInventoryTransaction
        {
            ItemId = itemId,
            Quantity = quantity,
            Type = EnumInventoryTransaction.In
        });
        
        stock.Quantity += quantity;
        
        int result = _db.SaveChanges();
        
        string message = result > 0 ? Message.Stock.StockInSuccess : Message.Stock.StockInFailed;
        Console.WriteLine(message);
    }

    public void StockOut(int itemId, int quantity)
    {
        var stock = _db.TblStocks.FirstOrDefault(x => x.ItemId == itemId);
        if (stock is null)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }

        if (stock.Quantity < quantity)
        {
            Console.WriteLine(Message.Stock.InsufficientStock);
            return;
        }
        
        stock.Quantity -= quantity;
        
        _db.TblInventoryTransactions.Add(new TblInventoryTransaction
        {
            ItemId = itemId,
            Quantity = quantity,
            Type = EnumInventoryTransaction.Out
        });
        
        int result = _db.SaveChanges();
        
        string message = result > 0 ? Message.Stock.StockOutSuccess : Message.Stock.StockOutFailed;
        Console.WriteLine(message);
    }
}