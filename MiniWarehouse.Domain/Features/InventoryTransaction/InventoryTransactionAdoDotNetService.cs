using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.AdoDotNet;

public class InventoryTransactionAdoDotNetService
{
    private readonly string _connectionString;

    public InventoryTransactionAdoDotNetService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public void StockIn(int itemId, int quantity)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        // Get current stock
        int? currentQty = null;
        
        string query = "SELECT Quantity FROM Tbl_Stocks WHERE ItemId = @ItemId";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@ItemId", itemId);
        
        var qty = cmd.ExecuteScalar();
        if (qty is not null)
        {
            currentQty = Convert.ToInt32(qty);
        }
        
        if (currentQty is null)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }
        
        // Insert transaction
        string query2 = @"
            INSERT INTO Tbl_InventoryTransactions (ItemId, Quantity, Type)
            VALUES (@ItemId, @Quantity, @Type)";
        
        SqlCommand cmd2 = new SqlCommand(query2, connection);
        cmd2.Parameters.AddWithValue("@ItemId", itemId);
        cmd2.Parameters.AddWithValue("@Quantity", quantity);
        cmd2.Parameters.AddWithValue("@Type", EnumInventoryTransaction.In.ToString());
        
        cmd2.ExecuteNonQuery();
        
        // Increase stock quantity
        string query3 = @"
            UPDATE Tbl_Stocks SET Quantity = Quantity + @Quantity
            WHERE ItemId = @ItemId";
        
        SqlCommand cmd3 = new SqlCommand(query3, connection);
        cmd3.Parameters.AddWithValue("@ItemId", itemId);
        cmd3.Parameters.AddWithValue("@Quantity", quantity);
        
        int result = cmd3.ExecuteNonQuery();
        
        string message = result > 0 ? Message.Stock.StockInSuccess : Message.Stock.StockInFailed;
        Console.WriteLine(message);
    }

    public void StockOut(int itemId, int quantity)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        // Get current stock
        int? currentQty = null;
        
        string query = "SELECT Quantity FROM Tbl_Stocks WHERE ItemId = @ItemId";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@ItemId", itemId);

        var qty = cmd.ExecuteScalar();
        if (qty is not null)
        {
            currentQty = Convert.ToInt32(qty);
        }

        if (currentQty is null)
        {
            Console.WriteLine(Message.Stock.NotFound);
            return;
        }

        if (currentQty < quantity)
        {
            Console.WriteLine(Message.Stock.InsufficientStock);
            return;
        }
        
        // Insert transaction
        string query2 = @"
            INSERT INTO Tbl_InventoryTransactions (ItemId, Type, Quantity)
            VALUES (@ItemId, @Type, @Quantity)";
        
        SqlCommand cmd2 = new SqlCommand(query2, connection);
        cmd2.Parameters.AddWithValue("@ItemId", itemId);
        cmd2.Parameters.AddWithValue("@Type", EnumInventoryTransaction.Out.ToString());
        cmd2.Parameters.AddWithValue("@Quantity", quantity);
        
        cmd2.ExecuteNonQuery();
        
        // Decrease stock quantity
        string updateQuery = @"
            UPDATE Tbl_Stocks SET Quantity = Quantity - @Quantity
            WHERE ItemId = @ItemId";
        
        SqlCommand cmd3 = new SqlCommand(updateQuery, connection);
        cmd3.Parameters.AddWithValue("@ItemId", itemId);
        cmd3.Parameters.AddWithValue("@Quantity", quantity);
        
        int result = cmd3.ExecuteNonQuery();
        
        string message = result > 0 ? Message.Stock.StockOutSuccess : Message.Stock.StockOutFailed;
        Console.WriteLine(message);
    }
}