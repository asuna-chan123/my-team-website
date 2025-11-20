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

    //public class CarouselViewModel
    //{
    //    public string Id { get; set; }          // "carousel-1"
    //    public string Title { get; set; }       // WHAT'S HOT...
    //    public IEnumerable<CarouselItemViewModel> Items { get; set; }
    //}

    //public class CarouselItemViewModel
    //{
    //    public string ImageUrl { get; set; }
    //    public string Title { get; set; }       // tên hiển thị
    //    public string Description { get; set; }
    //    public string Link { get; set; }
    //    public string Height { get; set; }
    //}
}

