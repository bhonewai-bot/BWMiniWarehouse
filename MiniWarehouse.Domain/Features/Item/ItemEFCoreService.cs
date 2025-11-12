using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.EFCore;

public class ItemEFCoreService
{
    private readonly AppDbContext _db;

    public ItemEFCoreService()
    {
        _db = new AppDbContext();
    }

    public List<ItemModel> ViewItems()
    {
        return _db.TblItems
            .AsNoTracking()
            .Select(x => new ItemModel()
            {
                ItemId = x.ItemId,
                Sku = x.Sku,
                ItemName = x.ItemName
            })
            .ToList();
    }

    public void AddItem(string sku, string itemName)
    {
        if (_db.TblItems.Any(x => x.Sku == sku))
        {
            Console.WriteLine(Message.Item.SkuAlreadyExists);
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
        
        string message = result > 0 ? Message.Item.AddedSuccessfully : Message.Item.AddFailed;
        Console.WriteLine(message);
    }
}