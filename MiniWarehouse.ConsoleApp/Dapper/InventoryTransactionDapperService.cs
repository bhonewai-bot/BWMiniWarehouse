using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;

namespace MiniWarehouse.ConsoleApp.Dapper;

public class InventoryTransactionDapperService
{
    private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
    {
        DataSource = ".",
        InitialCatalog = "MiniWarehouse",
        UserID = "sa",
        Password = "sasa@123",
        TrustServerCertificate = true
    };

    public void StockIn(int itemId, int quantity)
    {
        using (IDbConnection db = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString))
        {
            string query = "SELECT * FROM Tbl_Stocks WHERE ItemId = @ItemId";
            var stock = db.QueryFirstOrDefault<StockModel>(query, new { ItemId = itemId });
            
            if (stock is null)
            {
                Console.WriteLine("Stock not found.");
                return;
            }
            
            string insertQuery = @"
                INSERT INTO Tbl_InventoryTransactions (ItemId, Quantity, Type) 
                VALUES (@ItemId, @Quantity, @Type)";

            db.Execute(insertQuery, new InventoryTransactionModel()
            {
                ItemId = itemId,
                Type = EnumInventoryTransaction.In.ToString(),
                Quantity = quantity
            });

            string updateQuery = @"UPDATE Tbl_Stocks SET Quantity = Quantity + @Quantity WHERE ItemId = @ItemId";
            int result = db.Execute(updateQuery, new StockModel()
            {
                ItemId = itemId,
                Quantity = quantity,
            });
            
            string message = result > 0 ? "Stock In Success." : "Stock In Failed.";
            Console.WriteLine(message);
        }
    }

    public void StockOut(int itemId, int quantity)
    {
        using (IDbConnection db = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString))
        {
            string query = "SELECT * FROM Tbl_Stocks WHERE ItemId = @ItemId";
            var stock = db.QueryFirst<StockModel>(query, new { ItemId = itemId });
            
            if (stock is null)
            {
                Console.WriteLine("Stock not found.");
                return;
            }
            
            if (stock.Quantity <= 0 || stock.Quantity < quantity)
            {
                Console.WriteLine("Not enough stock.");
                return;
            }

            string insertQuery = @"
                INSERT INTO Tbl_InventoryTransactions (ItemId, Type, Quantity)
                VALUES (@ItemId, @Type, @Quantity)";

            db.Execute(insertQuery, new InventoryTransactionModel()
            {
                ItemId = itemId,
                Type = EnumInventoryTransaction.Out.ToString(),
                Quantity = quantity
            });

            string updateQuery = @"
                UPDATE Tbl_Stocks SET Quantity = Quantity - @Quantity
                WHERE ItemId = @ItemId";

            int result = db.Execute(updateQuery, new 
            {
                ItemId = itemId,
                Quantity = quantity,
            });
            
            string message = result > 0 ? "Stock Out Success." : "Stock Out Failed.";
            Console.WriteLine(message);
        }
    }
}