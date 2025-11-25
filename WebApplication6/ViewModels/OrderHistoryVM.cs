using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.ViewModels
{
    public class OrderHistoryVM
    {
        public int OrderID { get; set; }
        public DateTime? DateOrder { get; set; }
        public double Total { get; set; }
        public string ImagePro { get; set; }
        public string ProductName { get; set; }
    }
}