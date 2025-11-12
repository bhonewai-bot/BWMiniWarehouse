using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.Dapper;

public class ItemDapperService
{
    private readonly string _connectionString;

    public ItemDapperService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public List<ItemModel> ViewItems()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();

            string query = @"SELECT ItemId, SKU, ItemName FROM Tbl_Items";

            return db.Query<ItemModel>(query).ToList();
        }
    }

    public void AddItem( string sku, string itemName)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();

            // Validate SKU exists
            string query = @"SELECT * FROM Tbl_Items WHERE SKU = @SKU";
            
            var exists = db.Query<ItemModel>(query, new { SKU = sku }).FirstOrDefault();
            if (exists is not null)
            {
                Console.WriteLine(Message.Item.SkuAlreadyExists);
                return;
            }

            // Insert Item
            string insertItem = @"
                INSERT INTO Tbl_Items (SKU, ItemName)
                VALUES (@SKU, @ItemName)
                SELECT CAST(SCOPE_IDENTITY() as int)";

            var itemId = db.QuerySingle<int>(insertItem, new ItemModel()
            {
                Sku = sku,
                ItemName = itemName
            });
            
            // Insert Stock
            string insertStock = @"
                INSERT INTO Tbl_Stocks (ItemId, Quantity)
                VALUES (@ItemId, 0)";

            int result = db.Execute(insertStock, new StockModel()
            {
                ItemId = itemId
            });
            
            string message = result > 0 ? Message.Item.AddedSuccessfully : Message.Item.AddFailed;
            Console.WriteLine(message);
        }
    }
}