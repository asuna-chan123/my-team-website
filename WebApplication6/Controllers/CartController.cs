using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;
using WebApplication6.ViewModels;
using WebApplication6.ViewModels.CartViewModels;

namespace WebApplication6.Controllers
{
    public class CartController : Controller
    {
        private DBSportStoreEntities dbStore = new DBSportStoreEntities();

        //Hàm lấy dịch vụ giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // LATER FIX: [GET] Thêm vào giỏ hàng (Mặc định) - Cập nhật: Truyền tham số mặc định cho color/size
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
                product.Category.NameCate,
                "", "" // Default color/size
            );

            return RedirectToAction("Index");
        }

        // LATER FIX: [POST] Thêm vào giỏ hàng từ trang chi tiết (Nhận Color, Size, Quantity từ form)
        [HttpPost]
        public ActionResult AddToCart(int ProductID, string Color, string Size, int Quantity)
        {
            var product = dbStore.Products.Find(ProductID);
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
                Quantity,
                product.Category.NameCate,
                Color,
                Size
            );

            return RedirectToAction("Index");
        }

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

        // Hiển thị giỏ hàng
        [HttpGet]
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            
            // Populate Featured Products (Top 8)
            cart.FeaturedProducts = dbStore.Products.OrderByDescending(p => p.Price).Take(8).ToList();
            
            return View(cart);
        }

        // Dispose: Giải phóng tài nguyên kết nối database
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbStore.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}