using System.Data;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.AdoDotNet;

public class ItemAdoDotNetService
{
    private readonly string _connectionString;

    public ItemAdoDotNetService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public List<ItemModel> ViewItems()
    {
        var items = new List<ItemModel>();
        
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        string query = "SELECT ItemId, SKU, ItemName FROM Tbl_Items";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            items.Add(new ItemModel()
            {
                ItemId = Convert.ToInt32(reader["ItemId"]),
                ItemName = reader["ItemName"].ToString()!,
                Sku = reader["SKU"].ToString()!
            });
        }

        return items;
    }

    public void AddItem(string sku, string itemName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        // Validate SKU exists
        string query = "SELECT COUNT(*) FROM Tbl_Items WHERE SKU = @SKU";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@SKU", sku);
        
        int exists = (int)cmd.ExecuteScalar();
        if (exists > 0)
        {
            Console.WriteLine(Message.Item.SkuAlreadyExists);
            return;
        }
        
        // Insert Item
        string query2 = @"
            INSERT INTO Tbl_Items (SKU, ItemName)
            VALUES (@SKU, @ItemName)
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        SqlCommand cmd2 = new SqlCommand(query2, connection);
        cmd2.Parameters.AddWithValue("@SKU", sku);
        cmd2.Parameters.AddWithValue("@ItemName", itemName);

        int itemId = (int)cmd2.ExecuteScalar();

        var item = new ItemModel()
        {
            ItemId = itemId,
            Sku = sku,
            ItemName = itemName
        };
        
        // Insert Stock
        string query3 = @"
            INSERT INTO Tbl_Stocks (ItemId, Quantity)
            VALUES (@ItemId, 0)";
        
        SqlCommand cmd3 = new SqlCommand(query3, connection);
        cmd3.Parameters.AddWithValue("@ItemId", item.ItemId);
        
        int result = cmd3.ExecuteNonQuery();

        string message = result > 0 ? Message.Item.AddedSuccessfully : Message.Item.AddFailed;
        Console.WriteLine(message);
    }
}