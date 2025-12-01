using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.ViewModels
{
    public class ShippingOrderViewModel
    {
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; }

        // Customer Info
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        // Shipping Info
        public string ShippingAddress { get; set; }

        // Order Items
        public List<ShippingOrderItemViewModel> Items { get; set; }

        public decimal TotalAmount { get; set; }
    }

    public class ShippingOrderItemViewModel
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get { return Quantity * Price; } }
    }
}
