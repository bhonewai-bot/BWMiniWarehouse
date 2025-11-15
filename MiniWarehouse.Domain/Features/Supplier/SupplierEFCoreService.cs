using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.Domain.Features.Supplier;

public class SupplierEFCoreService
{
    private readonly AppDbContext _db;

    public SupplierEFCoreService()
    {
        _db = new AppDbContext();
    }

    public List<SupplierModel> ViewSuppliers()
    {
        return _db.TblSuppliers
            .AsNoTracking()
            .Select(x => new SupplierModel()
            {
                SupplierId = x.SupplierId,
                SupplierName = x.SupplierName
            })
            .ToList();
    }

    public void AddSupplier(string supplierName)
    {
        var supplier = new TblSupplier()
        {
            SupplierName = supplierName
        };
        
        _db.TblSuppliers.Add(supplier);
        int result = _db.SaveChanges();
        
        string message = result > 0 ? Message.Supplier.AddedSuccessfully : Message.Supplier.AddFailed;
        Console.WriteLine(message);
    }
}