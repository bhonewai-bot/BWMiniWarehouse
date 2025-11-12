using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;

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
                Quantity = x.Quantity
            })
            .ToList();
    }
}