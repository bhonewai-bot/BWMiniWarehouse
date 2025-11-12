namespace MiniWarehouse.Shared.Constants;

public class Message
{
    public static class Item
    {
        public const string AddedSuccessfully = "Item added successfully.";
        public const string AddFailed = "Item added failed.";
        public const string SkuAlreadyExists = "SKU already exists.";
        public const string NoItemsFound = "No items found.";
    }
    
    public static class Stock
    {
        public const string StockInSuccess = "Stock in successsful.";
        public const string StockInFailed = "Stock in failed.";
        public const string StockOutSuccess = "Stock out successful.";
        public const string StockOutFailed = "Stock out failed.";
        public const string NotFound = "Item not found.";
        public const string InsufficientStock = "Not enough stock.";
        public const string NoStocksFound = "Stocks not found.";
    }
    
    public static class Common
    {
        public const string InvalidInput = "Invalid input. Please try again.";
    }
}