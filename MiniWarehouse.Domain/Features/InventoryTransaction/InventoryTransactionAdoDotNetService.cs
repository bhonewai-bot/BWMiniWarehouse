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

    public void StockIn(int itemId, int quantity, int supplierId)
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
        
        // Get supplier
        string query4 = @"SELECT SupplierId, SupplierName FROM Tbl_Suppliers WHERE SupplierId = @SupplierId";
        
        SqlCommand cmd4 = new SqlCommand(query4, connection);
        cmd4.Parameters.AddWithValue("@SupplierId", supplierId);
        
        var supplier = cmd4.ExecuteScalar();
        if (supplier is null)
        {
            Console.WriteLine(Message.Supplier.NotFound);
            return;
        }
        
        // Insert transaction
        string query2 = @"
            INSERT INTO Tbl_InventoryTransactions (ItemId, Quantity, Type, SupplierId)
            VALUES (@ItemId, @Quantity, @Type, @SupplierId)";
        
        SqlCommand cmd2 = new SqlCommand(query2, connection);
        cmd2.Parameters.AddWithValue("@ItemId", itemId);
        cmd2.Parameters.AddWithValue("@Quantity", quantity);
        cmd2.Parameters.AddWithValue("@Type", EnumInventoryTransaction.In.ToString());
        cmd2.Parameters.AddWithValue("@SupplierId", supplierId);
        
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

    public void StockOut(int itemId, int quantity, int customerId)
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
        
        // Get customer
        string query4 = @"SELECT * FROM Tbl_Customers WHERE CustomerId = @CustomerId";
        SqlCommand cmd4 = new SqlCommand(query4, connection);
        cmd4.Parameters.AddWithValue("@CustomerId", customerId);
        
        var customer = cmd4.ExecuteScalar();
        if (customer is null)
        {
            Console.WriteLine(Message.Customer.NotFound);
        }
        
        // Insert transaction
        string query2 = @"
            INSERT INTO Tbl_InventoryTransactions (ItemId, Type, Quantity, CustomerId)
            VALUES (@ItemId, @Type, @Quantity, @CustomerId)";
        
        SqlCommand cmd2 = new SqlCommand(query2, connection);
        cmd2.Parameters.AddWithValue("@ItemId", itemId);
        cmd2.Parameters.AddWithValue("@Type", EnumInventoryTransaction.Out.ToString());
        cmd2.Parameters.AddWithValue("@Quantity", quantity);
        cmd2.Parameters.AddWithValue("@CustomerId", customerId);
        
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