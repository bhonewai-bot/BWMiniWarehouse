using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.Dapper;

public class StockDapperService
{
    private readonly string _connectionString;

    public StockDapperService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public List<StockModel> ViewStocks()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();
            
            string query = "SELECT ItemId, Quantity, RecorderLevel FROM Tbl_Stocks";
            
            return db.Query<StockModel>(query).ToList();
        }
    }

    public void SetReorderLevel(int itemId, int reorderLevel)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();
            
            // Validate stock exists
            string query = "SELECT * FROM Tbl_Stocks WHERE ItemId = @ItemId";
            
            var stock = db.QueryFirstOrDefault<StockModel>(query, new { ItemId = itemId });
            if (stock is null)
            {
                Console.WriteLine(Message.Stock.NotFound);
                return;
            }
            
            // Update reorder level
            string query2 = @"
                UPDATE Tbl_Stocks SET ReorderLevel = @ReorderLevel
                WHERE ItemId = @ItemId";
            
            int result = db.Execute(query2, new StockModel()
            {
                ItemId = itemId,
                ReorderLevel = reorderLevel
            });
            
            string message = result > 0 ? Message.Stock.SetRecorderLevelSuccess : Message.Stock.SetRecorderLevelFailed;
            Console.WriteLine(message);
        }
    }

    public void ExportStockToCsv()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();

            // Get stock with item inner join
            string query = @"
                SELECT s.StockId, s.ItemId, s.Quantity, s.ReorderLevel,
                    i.SKU, i.ItemName
                FROM Tbl_Stocks s
                INNER JOIN Tbl_Items i ON s.ItemId = i.ItemId";

            var stocks = db.Query<dynamic>(query).ToList();

            if (stocks.Count == 0)
            {
                Console.WriteLine("No stock data available to export.");
                return;
            }

            var csv = new StringBuilder();
            csv.AppendLine("Stock Id,SKU,Item name,Quantity,Reorder Level");
            
            foreach (var stock in stocks)
            {
                csv.AppendLine($"{stock.StockId},{stock.Sku},{stock.ItemName},{stock.Quantity},{stock.ReorderLevel}");
            }

            var fileName = $"StockReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var filePath = Path.Combine(downloadsPath, fileName);
            
            File.WriteAllText(filePath, csv.ToString());
        
            Console.WriteLine($"✅ Export successful! File saved at: {filePath}");
        }
    }
}