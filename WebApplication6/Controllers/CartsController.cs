using WebApplication6.Models;
using WebApplication6.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication6.ViewModels.CartViewModels;
using System.Data.Entity; // ← THÊM DÒNG NÀY ở đầu file

namespace WebApplication6.Controllers
{
    public class CartsController : Controller
    {
        public ActionResult ViewPage1()
        {
            return View();
        }
        //private readonly ApplicationDbContext db = new ApplicationDbContext();
        private DBSportStoreEntities dbStore = new DBSportStoreEntities();

        // Hàm lấy dịch vụ giỏ hàng
        private CartsService GetCartService()
        {
            return new CartsService(Session);
        }

        // Hiển thị giỏ hàng không gom nhóm theo danh mục
        [HttpGet]
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View();
        }

        // Thêm sản phẩm vào giỏ
        [HttpGet]
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = dbStore.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var cartService = GetCartService();
            cartService.GetCart().AddItem(
                product.ProductID,
                product.ImagePro,
                product.NamePro,
                product.Price ?? 0,
                quantity,
                product.Category.NameCate
            );

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        [HttpGet]
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        // Làm trống giỏ hàng
        [HttpGet]
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
       

       

    }
}