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

    public void StockIn(int itemId, int quantity, int supplierId)
    {
        var stock = _db.TblStocks.FirstOrDefault(x => x.ItemId == itemId);
        if (stock is null)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }
        
        var supplier = _db.TblSuppliers.FirstOrDefault(x => x.SupplierId == supplierId);
        if (supplier is null)
        {
            Console.WriteLine(Message.Supplier.NotFound);
            return;
        }

        _db.TblInventoryTransactions.Add(new TblInventoryTransaction
        {
            ItemId = itemId,
            Quantity = quantity,
            Type = EnumInventoryTransaction.In,
            SupplierId = supplierId
        });
        
        stock.Quantity += quantity;
        
        int result = _db.SaveChanges();
        
        string message = result > 0 ? Message.Stock.StockInSuccess : Message.Stock.StockInFailed;
        Console.WriteLine(message);
    }

    public void StockOut(int itemId, int quantity, int customerId)
    {
        var stock = _db.TblStocks.FirstOrDefault(x => x.ItemId == itemId);
        if (stock is null)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }
        
        var customer = _db.TblCustomers.FirstOrDefault(x => x.CustomerId == customerId);
        if (customer is null)
        {
            Console.WriteLine(Message.Customer.NotFound);
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
            Type = EnumInventoryTransaction.Out,
            CustomerId = customerId
        });
        
        int result = _db.SaveChanges();
        
        string message = result > 0 ? Message.Stock.StockOutSuccess : Message.Stock.StockOutFailed;
        Console.WriteLine(message);
    }
}