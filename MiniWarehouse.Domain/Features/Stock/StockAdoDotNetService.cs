using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;

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
        
        string query = "SELECT ItemId, Quantity FROM Tbl_Stocks";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            stock.Add(new StockModel()
            {
                ItemId = Convert.ToInt32(reader["ItemId"]),
                Quantity = Convert.ToInt32(reader["Quantity"])
            });
        }
        
        return stock;
    }
}