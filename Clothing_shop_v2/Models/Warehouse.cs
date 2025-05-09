using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Warehouse
{
    public int Id { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string? Location { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
}
