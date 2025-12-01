using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class ProductVariantViewModel
    {
        public int VariantID { get; set; }
        public int? ProductID { get; set; }

        public int? ColorID { get; set; }
        public int? SizeID { get; set; }

        [StringLength(100)]
        public string Sku { get; set; }

        public decimal? Price { get; set; }

        public int StockQty { get; set; }

        public List<VariantSizeStockViewModel> SizeStocks { get; set; }
    = new List<VariantSizeStockViewModel>();

        public List<string> ImageUrls { get; set; } = new List<string>();
        public string ImageUrl { get; set; }
    }
}
