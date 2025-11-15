using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.Domain.Features.Customer;

public class CustomerAdoDotNetService
{
    private readonly string _connectionString;

    public CustomerAdoDotNetService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public List<CustomerModel> ViewCustomers()
    {
        var customers = new List<CustomerModel>();
        
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        string query = "select CustomerId, CustomerName from Tbl_Customers";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            customers.Add(new CustomerModel()
            {
                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                CustomerName = reader["CustomerName"].ToString()!,
            });
        }
        
        return customers;
    }

    public void AddCustomer(string customerName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        // Insert Customer
        string query = @"
            INSERT INTO Tbl_Customers(CustomerName)
            VALUES (@CustomerName)";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@CustomerName", customerName);
        
        int result = cmd.ExecuteNonQuery();

        string message = result > 0 ? Message.Customer.AddedSuccessfully : Message.Customer.AddFailed;
        Console.WriteLine(message);
    }
}