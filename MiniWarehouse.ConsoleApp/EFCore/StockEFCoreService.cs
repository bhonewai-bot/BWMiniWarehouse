using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Database.AppDbContextModels;

namespace MiniWarehouse.ConsoleApp.EFCore;

public class StockEFCoreService
{
    private readonly AppDbContext _db;

    public StockEFCoreService(AppDbContext db)
    {
        _db = db;
    }

    public void ViewStock()
    {
        var lts = _db.TblStocks
            .AsNoTracking()
            .ToList();

        if (lts.Count == 0)
        {
            Console.WriteLine("No data found.");
        }

        foreach (var item in lts)
        {
            Console.WriteLine($"ItemId: {item.ItemId}, Quantity: {item.Quantity}");
        }
    }
}