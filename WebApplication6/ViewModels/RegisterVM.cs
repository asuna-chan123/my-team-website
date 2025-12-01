using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication6.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp!")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
    }
}