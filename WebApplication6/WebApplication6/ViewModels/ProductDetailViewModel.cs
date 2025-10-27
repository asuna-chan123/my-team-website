using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.ViewModels
{
    // WebApplication6.ViewModels.ProductDetailViewModel
    public class ProductDetailViewModel
    {
        public int ProductID { get; set; }
        public string NamePro { get; set; }
        public string DecriptionPro { get; set; }

        // DÙNG DECIMAL (khớp DB, tránh lỗi làm tròn)
        public decimal? Price { get; set; }

        public string ImagePro { get; set; }
        public string BannerVideoUrl { get; set; }
        public string FeatureImageUrl { get; set; }
        public string CategoryName { get; set; }
    }

}
