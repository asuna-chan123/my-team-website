using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<WebApplication6.Models.Product> Products { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }  // có thể là category name hoặc category id (chuỗi)
        public string PriceSort { get; set; }
        // thêm nếu muốn list category để render <select>
        public IEnumerable<Category> AllCategories { get; set; }

        public CarouselViewModel Carousel1 { get; set; }
        public CarouselViewModel Carousel2 { get; set; }
        //public IEnumerable<WebApplication6.Models.Category> AllCategories { get; set; }
    }
}

