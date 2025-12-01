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



        // Lấy thông tin sản phẩm (ProductVariant)  ProductID, Màu, Size
        [HttpPost]
        public ActionResult AddToCart(int ProductID, string Color, string Size, int Quantity)
        {
            // Kiểm tra sản phẩm có tồn tại không
            var product = dbStore.Products.Find(ProductID);
            if (product == null) return HttpNotFound();
            // Lấy thông tin variant dựa trên ProductID, Màu, Size
            var variant = dbStore.ProductVariants.FirstOrDefault(v =>
                v.ProductID == ProductID &&
                v.Color.ColorName == Color &&
                v.Size.SizeName == Size &&
                !v.IsDeleted);
            // Thêm sản phẩm vào giỏ hàng
            // Ưu tiên ảnh và số lượng từ variant, nếu không có thì lấy từ product
            GetCartService().GetCart().AddItem(
                ProductID,
                variant?.ImagePro ?? product.ImagePro,
                product.NamePro,
                product.Price ?? 0,
                Quantity,
                product.Category.NameCate,
                Color,
                Size,
                variant?.StockQty ?? 0
            );

            return RedirectToAction("Index");
        }
        //xóa sản phẩm khỏi giỏ hàng
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