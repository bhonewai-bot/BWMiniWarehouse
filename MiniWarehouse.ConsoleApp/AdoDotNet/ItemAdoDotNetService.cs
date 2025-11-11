using System.Data;
using Microsoft.Data.SqlClient;

namespace MiniWarehouse.ConsoleApp.AdoDotNet;

public class ItemAdoDotNetService
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
        SqlConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
        connection.Open();
        
        string query = "SELECT ItemId, SKU, ItemName FROM Tbl_Items";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            Console.WriteLine($"{reader["ItemId"]}: {reader["ItemName"]} (SKU: {reader["SKU"]})");
        }
        
        connection.Close();
    }

    public void AddItem(string itemName, string sku)
    {
        SqlConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
        connection.Open();
        
        string query = @"SELECT COUNT(*) FROM Tbl_Items WHERE SKU = @SKU";
        SqlCommand cmd = new SqlCommand(query, connection);
        
        cmd.Parameters.AddWithValue("@SKU", sku);
        int exists = (int)cmd.ExecuteScalar();
        if (exists > 0)
        {
            Console.WriteLine("SKU already exists.");
            return;
        }
        
        string insertItem = @"
            INSERT INTO Tbl_Items (ItemName, SKU) 
            VALUES (@ItemName, @SKU) SELECT CAST(SCOPE_IDENTITY() as int)";

        int itemId;
        
        SqlCommand cmd2 = new SqlCommand(insertItem, connection);
        cmd2.Parameters.AddWithValue("@ItemName", itemName);
        cmd2.Parameters.AddWithValue("@SKU", sku);
        
        itemId = (int)cmd2.ExecuteScalar();
        
        string insertStock = @"
            INSERT INTO Tbl_Stocks (ItemId, Quantity)
            VALUES (@ItemId, 0)";
        
        SqlCommand cmd3 = new SqlCommand(insertStock, connection);
        cmd3.Parameters.AddWithValue("@ItemId", itemId);
        int result = cmd3.ExecuteNonQuery();
        
        string message = result > 0 ? "Item added successfully." : "Item added failed.";
        Console.WriteLine(message);
        
        connection.Close();
    }
}