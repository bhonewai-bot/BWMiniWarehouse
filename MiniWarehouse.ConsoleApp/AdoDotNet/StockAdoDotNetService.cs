using Microsoft.Data.SqlClient;

namespace MiniWarehouse.ConsoleApp.AdoDotNet;

public class StockAdoDotNetService
{
    private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
    {
        DataSource = ".",
        InitialCatalog = "MiniWarehouse",
        UserID = "sa",
        Password = "sasa@123",
        TrustServerCertificate = true
    };

    public void ViewStock()
    {
        SqlConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
        connection.Open();
        
        string query = "SELECT ItemId, Quantity FROM Tbl_Stocks";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"ItemId: {reader["ItemId"]}, Quantity: {reader["Quantity"]}");
        }
        
        connection.Close();
    }
}