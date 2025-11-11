// See https://aka.ms/new-console-template for more information

using MiniWarehouse.ConsoleApp.AdoDotNet;using MiniWarehouse.ConsoleApp.Dapper;
using MiniWarehouse.ConsoleApp.EFCore;
using MiniWarehouse.Database.AppDbContextModels;

using var db = new AppDbContext();
var itemAdoDotNetService = new ItemAdoDotNetService();
var inventoryTransactionAdoDotNetService = new InventoryTransactionAdoDotNetService();
var stockAdoDotNetService = new StockAdoDotNetService();

var itemDapperService = new ItemDapperService();
var inventoryTransactionDapperService = new InventoryTransactionDapperService();
var stockDapperService = new StockDapperService();

var itemEfCoreService = new ItemEFCoreService(db);
var inventoryTransactionEfCoreService = new InventoryTransactionEFCoreService(db);
var stockEfCoreService = new StockEFCoreService(db);

while (true)
{
    Console.Clear();
    Console.WriteLine("\nWarehouse Management Menu");
    Console.WriteLine("1. View Items");
    Console.WriteLine("2. Add Item");
    Console.WriteLine("3. Stock In");
    Console.WriteLine("4. Stock Out");
    Console.WriteLine("5. View Stock");
    Console.WriteLine("0. Exit");
    Console.Write("Select an option: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1": 
            itemAdoDotNetService.ViewItems(); 
            return;
        case "2":
            Console.Write("Enter item name: ");
            string itemName = Console.ReadLine()!;
            Console.Write("Enter SKU: ");
            string sku = Console.ReadLine()!;
            itemAdoDotNetService.AddItem(itemName, sku); 
            return;
        case "3":
            Console.Write("Enter item id: ");
            int itemId = Convert.ToInt32(Console.ReadLine()!);
            Console.Write("Enter quantity: ");
            int quantity = Convert.ToInt32(Console.ReadLine()!);
            inventoryTransactionAdoDotNetService.StockIn(itemId, quantity);
            return;
        case "4":
            Console.Write("Enter item id: ");
            itemId = Convert.ToInt32(Console.ReadLine()!);
            Console.Write("Enter quantity: ");
            quantity = Convert.ToInt32(Console.ReadLine()!);
            inventoryTransactionAdoDotNetService.StockOut(itemId, quantity);
            return;
        case "5":
            stockAdoDotNetService.ViewStock();
            return;
        case "0": 
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            break;
    }
}