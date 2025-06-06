﻿using System;
using System.Collections.Generic;
using Clothing_shop_v2.Common.Models;

namespace Clothing_shop_v2.Models;

public partial class Promotion : BaseEntity
{
    //public int Id { get; set; }

    public string PromotionName { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    //public string? CreatedBy { get; set; }

    //public string? UpdatedBy { get; set; }

    //public DateTime? CreatedDate { get; set; }

    //public DateTime? UpdatedDate { get; set; }

    //public bool? IsActive { get; set; }

    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<VariantPromotion> VariantPromotions { get; set; } = new List<VariantPromotion>();
}
