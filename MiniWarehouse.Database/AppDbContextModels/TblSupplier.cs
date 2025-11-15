namespace MiniWarehouse.Database.AppDbContextModels;

public partial class TblSupplier
{
    public int SupplierId { get; set; }
    
    public string SupplierName { get; set; }
    
    public virtual ICollection<TblInventoryTransaction> TblInventoryTransactions { get; set; } = new List<TblInventoryTransaction>();
}