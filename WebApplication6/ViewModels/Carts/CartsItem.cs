using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.ViewModels.CartViewModels
{
    public class CartsItem
    {
        public int ProductID { get; set; }

        public string NamePro { get; set; }

        public string ImagePro { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string Category { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}