using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.Domain.Features.Supplier;

public class SupplierAdoDotNetService
{
    private readonly string _connectionString;

    public SupplierAdoDotNetService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public List<SupplierModel> ViewSuppliers()
    {
        var suppliers = new List<SupplierModel>();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        string query = "SELECT SupplierId, SupplierName FROM Tbl_Suppliers";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            suppliers.Add(new SupplierModel()
            {
                SupplierId = Convert.ToInt32(reader["SupplierId"]),
                SupplierName = reader["SupplierName"].ToString()!,
            });
        }
        
        return suppliers;
    }

    public void AddSupplier(string supplierName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        // Insert supplier
        string query = @"
            INSERT INTO Tbl_Suppliers (SupplierName)
            VALUES (@SupplierName)";
        
        SqlCommand cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@SupplierName", supplierName);
        
        int result = cmd.ExecuteNonQuery();

        string message = result > 0 ? Message.Supplier.AddedSuccessfully : Message.Supplier.AddFailed;
        Console.WriteLine(message);
    }
}