using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class InventoryTransaction
{
    public int Id { get; set; }

    public int VariantId { get; set; }

    public int WarehouseId { get; set; }

    public string TransactionType { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Reason { get; set; }

    public int? OrderId { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Variant Variant { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
