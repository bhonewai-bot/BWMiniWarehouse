using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;

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
            
            string query = "SELECT ItemId, Quantity FROM Tbl_Stocks";
            
            return db.Query<StockModel>(query).ToList();
        }
    }
}