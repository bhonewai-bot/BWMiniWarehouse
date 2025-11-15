using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.Domain.Features.Supplier;

public class SupplierDapperService
{
    private readonly string _connectionString;

    public SupplierDapperService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public List<SupplierModel> ViewSuppliers()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();
            
            string query = "SELECT SupplierId, SupplierName FROM Tbl_Suppliers";

            return db.Query<SupplierModel>(query).ToList();
        }
    }

    public void AddSupplier(string supplierName)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();

            // Insert Supplier
            string insertQuery = @"
                INSERT INTO Tbl_Suppliers (SupplierName)
                VALUES (@supplierName)";

            int result = db.Execute(insertQuery, new SupplierModel()
            {
                SupplierName = supplierName
            });

            string message = result > 0 ? Message.Supplier.AddedSuccessfully : Message.Supplier.AddFailed;
            Console.WriteLine(message);
        }
    }
}