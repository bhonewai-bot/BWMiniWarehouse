using System;
using System.Collections.Generic;

namespace MiniWarehouse.Database.AppDbContextModels;

public partial class TblInventoryTransaction
{
    public int TransactionId { get; set; }

    public int ItemId { get; set; }

    public EnumInventoryTransaction Type { get; set; }

    public int Quantity { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual TblItem Item { get; set; } = null!;
}

public enum EnumInventoryTransaction
{
    None,
    In,
    Out
}