using Microsoft.Data.SqlClient;
using MiniWarehouse.Database.AppDbContextModels;

namespace MiniWarehouse.ConsoleApp.AdoDotNet;

public class InventoryTransactionAdoDotNetService
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
        SqlConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
        connection.Open();
        
        string selectQuery = "SELECT Quantity FROM Tbl_Stocks WHERE ItemId = @ItemId";
        int? currentQty = null;
        
        SqlCommand cmd = new SqlCommand(selectQuery, connection);
        cmd.Parameters.AddWithValue("@ItemId", itemId);
        
        var qty = cmd.ExecuteScalar();
        if (qty is not null)
        {
            currentQty = Convert.ToInt32(qty);
        }

        if (currentQty is null)
        {
            Console.WriteLine("Stock not found.");
            return;
        }
        
        string insertQuery = @"
            INSERT INTO Tbl_InventoryTransactions (ItemId, Quantity, Type)
            VALUES (@ItemId, @Quantity, @Type)";
        
        SqlCommand cmd2 = new SqlCommand(insertQuery, connection);
        cmd2.Parameters.AddWithValue("@ItemId", itemId);
        cmd2.Parameters.AddWithValue("@Quantity", quantity);
        cmd2.Parameters.AddWithValue("@Type", EnumInventoryTransaction.In.ToString());
        cmd2.ExecuteNonQuery();
        
        string updateQuery = @"
            UPDATE Tbl_Stocks SET Quantity = Quantity + @Quantity
            WHERE ItemId = @ItemId";
        
        SqlCommand cmd3 = new SqlCommand(updateQuery, connection);
        cmd3.Parameters.AddWithValue("@ItemId", itemId);
        cmd3.Parameters.AddWithValue("@Quantity", quantity);
        
        int result = cmd3.ExecuteNonQuery();
        
        string message = result > 0 ? "Stock In Success." : "Stock In Failed.";
        Console.WriteLine(message);
        
        connection.Close();
    }

    public void StockOut(int itemId, int quantity)
    {
        SqlConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
        connection.Open();
        
        string selectQuery = "SELECT Quantity FROM Tbl_Stocks WHERE ItemId = @ItemId";
        int? currentQty = null;
        
        SqlCommand cmd = new SqlCommand(selectQuery, connection);
        cmd.Parameters.AddWithValue("@ItemId", itemId);

        var qty = cmd.ExecuteScalar();
        if (qty is not null)
        {
            currentQty = Convert.ToInt32(qty);
        }

        if (currentQty is null)
        {
            Console.WriteLine("Stock not found.");
            return;
        }

        if (currentQty <= 0 || currentQty < quantity)
        {
            Console.WriteLine("Not enough stock.");
            return;
        }
        
        string insertQuery = @"
            INSERT INTO Tbl_InventoryTransactions (ItemId, Type, Quantity)
            VALUES (@ItemId, @Type, @Quantity)";
        
        SqlCommand cmd2 = new SqlCommand(insertQuery, connection);
        cmd2.Parameters.AddWithValue("@ItemId", itemId);
        cmd2.Parameters.AddWithValue("@Type", EnumInventoryTransaction.Out.ToString());
        cmd2.Parameters.AddWithValue("@Quantity", quantity);
        cmd2.ExecuteNonQuery();
        
        string updateQuery = @"
            UPDATE Tbl_Stocks SET Quantity = Quantity - @Quantity
            WHERE ItemId = @ItemId";
        
        SqlCommand cmd3 = new SqlCommand(updateQuery, connection);
        cmd3.Parameters.AddWithValue("@ItemId", itemId);
        cmd3.Parameters.AddWithValue("@Quantity", quantity);
        
        int result = cmd3.ExecuteNonQuery();
        
        string message = result > 0 ? "Stock Out Success." : "Stock Out Failed.";
        Console.WriteLine(message);
        
        connection.Close();
    }
}