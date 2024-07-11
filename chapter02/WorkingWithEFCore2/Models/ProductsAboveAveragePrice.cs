using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkingWithEFCore2.Models;

[Keyless] //Declara q as linhas retornada vem de uma view 
public partial class ProductsAboveAveragePrice
{
    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal? UnitPrice { get; set; }
}
