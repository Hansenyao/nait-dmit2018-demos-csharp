﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ExampleMudSystem.Entities;

public partial class Invoice
{
    public int InvoiceID { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public int CustomerID { get; set; }

    public int EmployeeID { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Tax { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}