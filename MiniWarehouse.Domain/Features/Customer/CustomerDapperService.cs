using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.Domain.Features.Customer;

public class CustomerDapperService
{
    private readonly string _connectionString;

    public CustomerDapperService()
    {
        _connectionString = AppSettings.ConnectionString;
    }
    
    public List<CustomerModel> ViewCustomers()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();
            
            string query = "SELECT CustomerId, CustomerName FROM Tbl_Customers";

            return db.Query<CustomerModel>(query).ToList();
        }
    }

    public void AddCustomer(string customerName)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();
            
            // Insert Customer
            string insertQuery = @"
                INSERT INTO Tbl_Customers(CustomerName)
                VALUES (@customerName)";

            int result = db.Execute(insertQuery, new CustomerModel()
            {
                CustomerName = customerName
            });

            string message = result > 0 ? Message.Customer.AddedSuccessfully : Message.Customer.AddFailed;
            Console.WriteLine(message);
        }
    }
}