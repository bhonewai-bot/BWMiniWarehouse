// See https://aka.ms/new-console-template for more information

/*using MiniWarehouse.ConsoleApp;

var menu = new Menu();
menu.Run();*/

using MiniWarehouse.ConsoleApp.AdoDotNet;
using MiniWarehouse.ConsoleApp.Dapper;
using MiniWarehouse.ConsoleApp.EFCore;

var itemAdoDotNetService = new ItemAdoDotNetService();
var inventoryTransactionAdoDotNetService = new InventoryTransactionAdoDotNetService();
var stockAdoDotNetService = new StockAdoDotNetService();

var itemDapperService = new ItemDapperService();
var inventoryTransactionDapperService = new InventoryTransactionDapperService();
var stockDapperService = new StockDapperService();

var itemEFCoreService = new ItemEFCoreService();
var inventoryTransactionEFCoreService = new InventoryTransactionEFCoreService();
var stockEFCoreService = new StockEFCoreService();

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
        Console.WriteLine("No items found");
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
        
    inventoryTransactionEFCoreService.StockIn(itemId, quantity);
}

void StockOut()
{
    Console.Write("Enter Item ID: ");
    int itemId = Convert.ToInt32(Console.ReadLine());
        
    Console.Write("Enter quantity: ");
    int quantity = Convert.ToInt32(Console.ReadLine());
        
    inventoryTransactionEFCoreService.StockOut(itemId, quantity);
}

void ViewStocks()
{
    var stocks = stockEFCoreService.ViewStocks();

    if (stocks.Count == 0)
    {
        Console.WriteLine("No stock found");
    }

    foreach (var stock in stocks)
    {
        Console.WriteLine($"ItemId: {stock.ItemId}, Quantity: {stock.Quantity}");
    }
}