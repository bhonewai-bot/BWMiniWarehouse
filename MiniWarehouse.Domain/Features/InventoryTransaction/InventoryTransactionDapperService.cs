using System.Data;
using System.Net.WebSockets;
using Dapper;
using Microsoft.Data.SqlClient;
using MiniWarehouse.Database;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.ConsoleApp.Dapper;

public class InventoryTransactionDapperService
{
    private readonly string _connectionString;

    public InventoryTransactionDapperService()
    {
        _connectionString = AppSettings.ConnectionString;
    }

    public void StockIn(int itemId, int quantity, int supplierId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();
            
            // Get current stock
            string query = "SELECT * FROM Tbl_Stocks WHERE ItemId = @ItemId";
            
            var stock = db.QueryFirstOrDefault<StockModel>(query, new { ItemId = itemId });
            if (stock is null)
            {
                Console.WriteLine(Message.Stock.NotFound);
                return;
            }
            
            // Get supplier
            string query4 = @"SELECT SupplierId, SupplierName FROM Tbl_Suppliers WHERE SupplierId = @SupplierId";
            
            var supplier = db.QueryFirstOrDefault<SupplierModel>(query4, new { SupplierId = supplierId });
            if (supplier is null)
            {
                Console.WriteLine(Message.Supplier.NotFound);
                return;
            }
            
            // Insert transaction
            string query2 = @"
                INSERT INTO Tbl_InventoryTransactions (ItemId, Quantity, Type, SupplierId) 
                VALUES (@ItemId, @Quantity, @Type, @SupplierId)";

            db.Execute(query2, new InventoryTransactionModel()
            {
                ItemId = itemId,
                Type = EnumInventoryTransaction.In.ToString(),
                Quantity = quantity,
                SupplierId = supplierId
            });

            // Increase stock quantity
            string query3 = @"
                UPDATE Tbl_Stocks SET Quantity = Quantity + @Quantity 
                WHERE ItemId = @ItemId";
            int result = db.Execute(query3, new StockModel()
            {
                ItemId = itemId,
                Quantity = quantity,
            });
            
            string message = result > 0 ? Message.Stock.StockInSuccess : Message.Stock.StockInFailed;
            Console.WriteLine(message);
        }
    }

    public void StockOut(int itemId, int quantity, int customerId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            db.Open();
            
            // Get current stock
            string query = "SELECT * FROM Tbl_Stocks WHERE ItemId = @ItemId";
            
            var stock = db.QueryFirst<StockModel>(query, new { ItemId = itemId });
            if (stock is null)
            {
                Console.WriteLine(Message.Stock.NotFound);
                return;
            }
            
            if (stock.Quantity < quantity)
            {
                Console.WriteLine(Message.Stock.InsufficientStock);
                return;
            }
            
            // Get customer
            string query4 = @"SELECT * FROM Tbl_Customers WHERE CustomerId = @CustomerId";
            var customer = db.QueryFirstOrDefault<CustomerModel>(query4, new { CustomerId = customerId });
            if (customer is null)
            {
                Console.WriteLine(Message.Customer.NotFound);
                return;
            }

            // Insert transaction
            string insertQuery = @"
                INSERT INTO Tbl_InventoryTransactions (ItemId, Type, Quantity, CustomerId)
                VALUES (@ItemId, @Type, @Quantity, @CustomerId)";

            db.Execute(insertQuery, new InventoryTransactionModel()
            {
                ItemId = itemId,
                Type = EnumInventoryTransaction.Out.ToString(),
                Quantity = quantity,
                CustomerId = customerId
            });

            // Decrease stock quantity
            string updateQuery = @"
                UPDATE Tbl_Stocks SET Quantity = Quantity - @Quantity
                WHERE ItemId = @ItemId";

            int result = db.Execute(updateQuery, new 
            {
                ItemId = itemId,
                Quantity = quantity,
            });
            
            string message = result > 0 ? Message.Stock.StockOutSuccess : Message.Stock.StockOutFailed;
            Console.WriteLine(message);
        }
    }
}