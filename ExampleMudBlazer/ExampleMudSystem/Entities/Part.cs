﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ExampleMudSystem.Entities;

public partial class Part
{
    public int PartID { get; set; }

    public int PartCategoryID { get; set; }

    public string Description { get; set; }

    public decimal Cost { get; set; }

    public decimal Price { get; set; }

    public int ROL { get; set; }

    public int QOH { get; set; }

    public bool Taxable { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

    public virtual Lookup PartCategory { get; set; }
}