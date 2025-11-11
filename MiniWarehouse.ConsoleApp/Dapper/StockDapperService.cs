using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Domain.Models;

namespace MiniWarehouse.ConsoleApp.Dapper;

public class StockDapperService
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
        using (IDbConnection db = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString))
        {
            string query = "SELECT ItemId, Quantity FROM Tbl_Stocks";
            List<StockModel> lts = db.Query<StockModel>(query).ToList();
            
            if (lts.Count == 0)
            {
                Console.WriteLine("No data found.");
            }

            foreach (var item in lts)
            {
                Console.WriteLine($"ItemId: {item.ItemId}, Quantity: {item.Quantity}");
            }
        }
    }
}