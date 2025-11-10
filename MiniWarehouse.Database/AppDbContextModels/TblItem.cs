using System;
using System.Collections.Generic;

namespace MiniWarehouse.Database.AppDbContextModels;

public partial class TblItem
{
    public int ItemId { get; set; }

    public string Sku { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TblInventoryTransaction> TblInventoryTransactions { get; set; } = new List<TblInventoryTransaction>();

    public virtual ICollection<TblStock> TblStocks { get; set; } = new List<TblStock>();
}
