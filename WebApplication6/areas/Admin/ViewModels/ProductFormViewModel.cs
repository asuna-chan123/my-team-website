using System.Collections.Generic;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class ProductFormViewModel
    {
        public Product Product { get; set; }
        public List<ProductVariantViewModel> Variants { get; set; }

        public SelectList ColorList { get; set; }
        public SelectList SizeList { get; set; }

        public ProductFormViewModel()
        {
            Variants = new List<ProductVariantViewModel>();
        }
    }
}
