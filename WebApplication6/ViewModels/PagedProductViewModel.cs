using PagedList;
using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class PagedProductViewModel
    {
        public IPagedList<Product> Products { get; set; }

        public int? Category { get; set; }
        public string SearchString { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
