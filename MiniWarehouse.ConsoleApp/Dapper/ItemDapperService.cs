using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;

namespace MiniWarehouse.ConsoleApp.Dapper;

public class ItemDapperService
{
    private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
    {
        DataSource = ".",
        InitialCatalog = "MiniWarehouse",
        UserID = "sa",
        Password = "sasa@123",
        TrustServerCertificate = true
    };

    public void ViewItems()
    {
        using (IDbConnection db = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString))
        {
            db.Open();

            string query = @"SELECT ItemId, SKU, ItemName FROM Tbl_Items";

            List<ItemModel> lts = db.Query<ItemModel>(query).ToList();

            foreach (var item in lts)
            {
                Console.WriteLine($"{item.ItemId}: {item.ItemName} (SKU: {item.Sku})");
            }
        }
    }

    public void AddItem(string itemName, string sku)
    {
        using (IDbConnection db = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString))
        {
            db.Open();

            string query = @"SELECT * FROM Tbl_Items WHERE SKU = @SKU";
            var exists = db.Query<ItemModel>(query, new { SKU = sku }).FirstOrDefault();

            if (exists is not null)
            {
                Console.WriteLine("SKU already exists.");
                return;
            }

            string insertItem = @"
                INSERT INTO Tbl_Items (ItemName, SKU) 
                VALUES (@ItemName, @SKU) SELECT CAST(SCOPE_IDENTITY() as int)";

            var itemId = db.QuerySingle<int>(insertItem, new ItemModel()
            {
                ItemName = itemName,
                Sku = sku
            });
            
            string insertStock = @"
                INSERT INTO Tbl_Stocks (ItemId, Quantity)
                VALUES (@ItemId, 0)";

            int result = db.Execute(insertStock, new StockModel()
            {
                ItemId = itemId
            });
            
            string message = result > 0 ? "Item added successfully." : "Item added failed.";
            Console.WriteLine(message);
        }
    }
}