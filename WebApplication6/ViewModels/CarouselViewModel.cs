using System.Collections.Generic;
using PagedList;
using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class CarouselViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<CarouselItemViewModel> Items { get; set; }
    }

    public class CarouselItemViewModel
    {
        // để khớp với JS hiện tại:
        public string img { get; set; }
        public string price { get; set; }
        public string desc { get; set; }
        public string height { get; set; }
        public string link { get; set; }
    }
}

