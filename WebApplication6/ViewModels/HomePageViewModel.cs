using System.Collections.Generic;
using PagedList;
using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class HomeProductListViewModel : PagedProductViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public CarouselViewModel Carousel1 { get; set; }
        public CarouselViewModel Carousel2 { get; set; }
    }
}

