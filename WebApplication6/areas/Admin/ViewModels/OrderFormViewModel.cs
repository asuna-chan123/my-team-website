using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class OrderFormViewModel
    {
        public OrderPro Order { get; set; }
        public List<OrderDetail> Details { get; set; }


        public SelectList CustomerList { get; set; }
        public SelectList ProductList { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0\d{9}|\+\d{9,14})$",
            ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 (10 số) hoặc dạng +XXXXXXXX (tối đa 15 số)")]
        [MaxLength(15)]
        public string CustomerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string CustomerEmail { get; set; }

        public string CustomerUserName { get; set; }

        public int? ExistingCustomerId { get; set; }
        public OrderFormViewModel()
        {
            Details = new List<OrderDetail>
        {
            new OrderDetail()   // hàng đầu tiên
        };
        }
    }
}

