using System.Text;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.AdoDotNet;

public class StockAdoDotNetService
{
    private readonly string _connectionString;

    public StockAdoDotNetService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public List<StockModel> ViewStocks()
    {
        var stock = new List<StockModel>();
        
        SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        
        string query = "SELECT ItemId, Quantity, RecorderLevel FROM Tbl_Stocks";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            stock.Add(new StockModel()
            {
                ItemId = Convert.ToInt32(reader["ItemId"]),
                Quantity = Convert.ToInt32(reader["Quantity"]),
                ReorderLevel = Convert.ToInt32(reader["RecorderLevel"])
            });
        }
        
        return stock;
    }
    
    public void SetReorderLevel(int itemId, int reorderLevel)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        // Validate item exists
        string query = "SELECT COUNT(*) FROM Tbl_Stocks WHERE ItemId = @ItemId";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@ItemId", itemId);
        
        int exists = (int)cmd.ExecuteScalar();
        if (exists == 0)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }
        
        // Update reorder level
        string query2 = @"
            UPDATE Tbl_Stocks SET ReorderLevel = @ReorderLevel
            WHERE ItemId = @ItemId";
        
        SqlCommand cmd2 = new SqlCommand(query2, connection);
        cmd2.Parameters.AddWithValue("@ItemId", itemId);
        cmd2.Parameters.AddWithValue("@ReorderLevel", reorderLevel);
        
        int result = cmd2.ExecuteNonQuery();
        
        string message = result > 0 ? Message.Stock.SetRecorderLevelSuccess : Message.Stock.SetRecorderLevelFailed;
        Console.WriteLine(message);
    }

    public void ExportStockToCsv()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        // Get stock with item inner join
        string query = @"
            SELECT s.StockId, s.ItemId, s.Quantity, s.ReorderLevel,
                i.SKU, i.ItemName
            FROM Tbl_Stocks s
            INNER JOIN Tbl_Items i ON s.ItemId = i.ItemId";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();
        
        var csv = new StringBuilder();
        csv.AppendLine("Stock Id,SKU,Item name,Quantity,Reorder Level");

        int count = 0;
        while (reader.Read())
        {
            int stockId = Convert.ToInt32(reader["StockId"]);
            string sku = reader["SKU"].ToString()!;
            string itemName = reader["ItemName"].ToString()!;
            int quantity = Convert.ToInt32(reader["Quantity"]);
            int reorderLevel = Convert.ToInt32(reader["ReorderLevel"]);
            
            csv.AppendLine($"{stockId},{sku},{itemName},{quantity},{reorderLevel}");
            count++;
        }

        if (count == 0)
        {
            Console.WriteLine("No stock data available to export.");
            return;
        }
        
        var fileName = $"StockReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        var filePath = Path.Combine(downloadsPath, fileName);
            
        File.WriteAllText(filePath, csv.ToString());
        
        Console.WriteLine($"✅ Export successful! File saved at: {filePath}");
    }
}