using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication6.ViewModels.CartViewModels; // Ensure this namespace is correct

namespace WebApplication6.ViewModels.Cart
{
    public class OrderItemViewModel
    {
        public int ProductID { get; set; }
        public string NamePro { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}