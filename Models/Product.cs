﻿using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? ProductName { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCatId { get; set; }

    public int? BrandId { get; set; }

    public string? Price { get; set; }

    public string? Description { get; set; }

    public string? DiscountDescription { get; set; }

    public string? InboxDescription { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }

    public int? Quantity { get; set; }

    public string? Status { get; set; }

    public string? Frontavatar { get; set; }

    public string? Backavatar { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual SubCategory? SubCat { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}