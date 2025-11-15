namespace MiniWarehouse.Database.AppDbContextModels;

public partial class TblCustomer
{
    public int CustomerId { get; set; }
    
    public string CustomerName { get; set; }
    
    public virtual ICollection<TblInventoryTransaction> TblInventoryTransactions { get; set; } = new List<TblInventoryTransaction>();
}