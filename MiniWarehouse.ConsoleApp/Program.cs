// See https://aka.ms/new-console-template for more information

using MiniWarehouse.ConsoleApp.EFCore;
using MiniWarehouse.Database.AppDbContextModels;

Console.WriteLine("Hello, World!");

using var db = new AppDbContext();
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
            itemEfCoreService.ViewItems(); 
            return;
        case "2":
            Console.Write("Enter item name: ");
            string itemName = Console.ReadLine()!;
            Console.Write("Enter SKU: ");
            string sku = Console.ReadLine()!;
            itemEfCoreService.AddItem(itemName, sku); 
            return;
        case "3":
            Console.Write("Enter item id: ");
            int itemId = Convert.ToInt32(Console.ReadLine()!);
            Console.Write("Enter quantity: ");
            int quantity = Convert.ToInt32(Console.ReadLine()!);
            inventoryTransactionEfCoreService.StockIn(itemId, quantity);
            return;
        case "4":
            Console.Write("Enter item id: ");
            itemId = Convert.ToInt32(Console.ReadLine()!);
            Console.Write("Enter quantity: ");
            quantity = Convert.ToInt32(Console.ReadLine()!);
            inventoryTransactionEfCoreService.StockOut(itemId, quantity);
            return;
        case "5":
            stockEfCoreService.ViewStock();
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