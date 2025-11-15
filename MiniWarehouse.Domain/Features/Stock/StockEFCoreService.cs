using System.Text;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.EFCore;

public class StockEFCoreService
{
    private readonly AppDbContext _db;

    public StockEFCoreService()
    {
        _db = new AppDbContext();
    }

    public List<StockModel> ViewStocks()
    {
        return _db.TblStocks
            .AsNoTracking()
            .Select(x => new StockModel()
            {
                ItemId = x.ItemId,
                Quantity = x.Quantity,
                ReorderLevel = x.ReorderLevel
            })
            .ToList();
    }

    public void SetReorderLevel(int itemId, int recorderLevel)
    {
        var stock = _db.TblStocks.FirstOrDefault(x => x.ItemId == itemId);
        if (stock is null)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }
        
        stock.ReorderLevel = recorderLevel;
        
        int result = _db.SaveChanges();
        
        string message = result > 0 ? Message.Stock.SetRecorderLevelSuccess : Message.Stock.SetRecorderLevelFailed;
        Console.WriteLine(message);
    }

    public void ExportStockToCsv()
    {
        var stocks = _db.TblStocks
            .AsNoTracking()
            .Include(x => x.Item)
            .ToList();

        if (stocks.Count == 0)
        {
            Console.WriteLine("No stock data available to export.");
            return;
        }

        var csv = new StringBuilder();
        csv.AppendLine("Stock Id,SKU,Item name,Quantity,Reorder Level");

        foreach (var stock in stocks)
        {
            csv.AppendLine($"{stock.StockId},{stock.Item.Sku},{stock.Item.ItemName},{stock.Quantity},{stock.ReorderLevel}");
        }

        var fileName = $"StockReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        var filePath = Path.Combine(downloadsPath, fileName);
        
        File.WriteAllText(filePath, csv.ToString());
        
        Console.WriteLine($"✅ Export successful! File saved at: {filePath}");
    }
}