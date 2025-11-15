// See https://aka.ms/new-console-template for more information

/*using MiniWarehouse.ConsoleApp;

var menu = new Menu();
menu.Run();*/

using MiniWarehouse.ConsoleApp.AdoDotNet;
using MiniWarehouse.ConsoleApp.Dapper;
using MiniWarehouse.ConsoleApp.EFCore;
using MiniWarehouse.Domain.Features.Customer;
using MiniWarehouse.Domain.Features.Supplier;

var itemAdoDotNetService = new ItemAdoDotNetService();
var inventoryTransactionAdoDotNetService = new InventoryTransactionAdoDotNetService();
var stockAdoDotNetService = new StockAdoDotNetService();
var supplierAdoDotNetService = new SupplierAdoDotNetService();
var customerAdoDotNetService = new CustomerAdoDotNetService();

var itemDapperService = new ItemDapperService();
var inventoryTransactionDapperService = new InventoryTransactionDapperService();
var stockDapperService = new StockDapperService();
var supplierDapperService = new SupplierDapperService();
var customerDapperService = new CustomerDapperService();

var itemEFCoreService = new ItemEFCoreService();
var inventoryTransactionEFCoreService = new InventoryTransactionEFCoreService();
var stockEFCoreService = new StockEFCoreService();
var supplierEFCoreService = new SupplierEFCoreService();
var customerEFCoreService = new CustomerEFCoreService();

while (true)
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("  Warehouse Management System");
    Console.WriteLine("=================================");
    Console.WriteLine("1. View Items");
    Console.WriteLine("2. Add Item");
    Console.WriteLine("3. Stock In");
    Console.WriteLine("4. Stock Out");
    Console.WriteLine("5. View Stocks");
    Console.WriteLine("6. Set Reorder Level");
    Console.WriteLine("7. View Suppliers");
    Console.WriteLine("8. Add Supplier");
    Console.WriteLine("9. View Customers");
    Console.WriteLine("10. Add Customer");
    Console.WriteLine("11. Export Stock to CSV");
    Console.WriteLine("0. Exit");
    Console.WriteLine("=================================");
    Console.Write("Select an option: ");
    var choice = Console.ReadLine();
    
    switch (choice)
    {
        case "1":
            ViewItems();
            return;
        case "2":
            AddItem();
            return;
        case "3":
            StockIn();
            return;
        case "4":
            StockOut();
            return;
        case "5":
            ViewStocks();
            return;
        case "6":
            SetReorderLevel();
            return;
        case "7":
            ViewSuppliers();
            return;
        case "8":
            AddSupplier();
            return;
        case "9":
            ViewCustomers();
            return;
        case "10":
            AddCustomer();
            return;
        case "11":
            ExportStockToCsv();
            return;
        case "0": 
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            Console.WriteLine("\nPress any key to continue...");
            break;
    }
}

void ViewItems()
{
    var items = itemEFCoreService.ViewItems();

    if (items.Count == 0)
    {
        Console.WriteLine("No items found.");
    }

    foreach (var item in items)
    {
        Console.WriteLine($"{item.ItemId}: {item.ItemName} (SKU: {item.Sku})");
    }
}

void AddItem()
{
    Console.Write("Enter SKU: ");
    string sku = Console.ReadLine() ?? string.Empty;
        
    Console.Write("Enter item name: ");
    string itemName = Console.ReadLine() ?? string.Empty;
        
    itemEFCoreService.AddItem(sku, itemName);
}

void StockIn()
{
    Console.Write("Enter Item ID: ");
    int itemId = Convert.ToInt32(Console.ReadLine());
        
    Console.Write("Enter quantity: ");
    int quantity = Convert.ToInt32(Console.ReadLine());
    
    Console.Write("Enter supplier ID: ");
    int supplierId = Convert.ToInt32(Console.ReadLine());
        
    inventoryTransactionDapperService.StockIn(itemId, quantity, supplierId);
}

void StockOut()
{
    Console.Write("Enter Item ID: ");
    int itemId = Convert.ToInt32(Console.ReadLine());
        
    Console.Write("Enter quantity: ");
    int quantity = Convert.ToInt32(Console.ReadLine());
    
    Console.Write("Enter customer ID: ");
    int customerId = Convert.ToInt32(Console.ReadLine());
        
    inventoryTransactionDapperService.StockOut(itemId, quantity, customerId);
}

void ViewStocks()
{
    var stocks = stockEFCoreService.ViewStocks();

    if (stocks.Count == 0)
    {
        Console.WriteLine("No stocks found.");
    }

    foreach (var stock in stocks)
    {
        Console.WriteLine($"ItemId: {stock.ItemId}, Quantity: {stock.Quantity}, RecorderLevel: {stock.ReorderLevel}");
        
        if(stock.Quantity <= stock.ReorderLevel)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠️ Low stock alert: Item ID {stock.ItemId} (Qty: {stock.Quantity})");
            Console.ResetColor();
        }
    }
}

void SetReorderLevel()
{
    Console.Write("Enter Item ID: ");
    int itemId = Convert.ToInt32(Console.ReadLine());
        
    Console.Write("Enter Recorder Level: ");
    int recorderLevel = Convert.ToInt32(Console.ReadLine());
    
    stockAdoDotNetService.SetReorderLevel(itemId, recorderLevel);
}

void ViewSuppliers()
{
    var suppliers = supplierAdoDotNetService.ViewSuppliers();

    if (suppliers.Count == 0)
    {
        Console.WriteLine("No suppliers found.");
    }

    foreach (var supplier in suppliers)
    {
        Console.WriteLine($"{supplier.SupplierId}: {supplier.SupplierName}");
    }
}

void AddSupplier()
{
    Console.Write("Enter supplier name: ");
    string supplierName = Console.ReadLine() ?? string.Empty;
    
    supplierAdoDotNetService.AddSupplier(supplierName);
}

void ViewCustomers()
{
    var customers = customerDapperService.ViewCustomers();

    if (customers.Count == 0)
    {
        Console.WriteLine("No customers found.");
    }

    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.CustomerId}: {customer.CustomerName}");
    }
}

void AddCustomer()
{
    Console.Write("Enter customer name: ");
    string customerName = Console.ReadLine() ?? string.Empty;
    
    customerDapperService.AddCustomer(customerName);
}

void ExportStockToCsv()
{
    stockAdoDotNetService.ExportStockToCsv();
}