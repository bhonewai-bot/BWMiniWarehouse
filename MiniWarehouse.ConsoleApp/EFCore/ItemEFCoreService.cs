using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Database.AppDbContextModels;

namespace MiniWarehouse.ConsoleApp.EFCore;

public class ItemEFCoreService
{
    private readonly AppDbContext _db;

    public ItemEFCoreService(AppDbContext db)
    {
        _db = db;
    }

    public void ViewItems()
    {
        var lts = _db.TblItems
            .AsNoTracking()
            .ToList();

        if (lts.Count == 0)
        {
            Console.WriteLine("No data found");
        }

        foreach (var item in lts)
        {
            Console.WriteLine($"{item.ItemId}: {item.ItemName} (SKU: {item.Sku})");
        }
    }

    public void AddItem(string itemName, string sku)
    {
        if (_db.TblItems.Any(x => x.Sku == sku))
        {
            Console.WriteLine("Sku already exists");
            return;
        }

        var item = new TblItem()
        {
            ItemName = itemName,
            Sku = sku
        };
        _db.TblItems.Add(item);
        _db.SaveChanges();
        
        _db.TblStocks.Add(new TblStock()
        {
            ItemId = item.ItemId,
            Quantity = 0
        });
        int result = _db.SaveChanges();
        
        string message = result > 0 ? "Item added successfully." : "Item added failed.";
        Console.WriteLine(message);
    }
}