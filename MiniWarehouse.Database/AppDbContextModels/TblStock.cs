using System;
using System.Collections.Generic;

namespace MiniWarehouse.Database.AppDbContextModels;

public partial class TblStock
{
    public int StockId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public int ReorderLevel { get; set; }

    public virtual TblItem Item { get; set; } = null!;
}
