using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication6.Models;
using PagedList.Mvc;
using PagedList;
using System.Web.UI.WebControls;


namespace WebApplication6.ViewModels.CartViewModels
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items => items;

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Danh sách các sản phẩm cùng danh mục với các sản phẩm trong giỏ
        public PagedList.IPagedList<Product> SimilarProducts { get; set; }

        // Featured products to display in cart view
        public IEnumerable<Product> FeaturedProducts { get; set; }

        // Grouped items by category (computed property)
        public List<IGrouping<string, CartItem>> GroupedItems => items.GroupBy(i => i.Category).ToList();

        // LATER FIX: Thêm sản phẩm vào giỏ (Cập nhật: Thêm tham số color, size để xử lý biến thể)
        public void AddItem(int productId, string productImage, string productName,
                           decimal unitPrice, int quantity, string category, string color, string size)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == productId && i.Color == color && i.Size == size);

            if (existingItem == null)
            {
                items.Add(new CartItem
                {
                    ProductID = productId,
                    ImagePro = productImage,
                    NamePro = productName,
                    UnitPrice = unitPrice,
                    Quantity = quantity,
                    Category = category,
                    Color = color,
                    Size = size
                });
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }

        // Xóa sản phẩm khỏi giỏ
        public void RemoveItem(int productId)
        {
            items.RemoveAll(i => i.ProductID == productId);
        }

        // Tính tổng giá trị giỏ hàng
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        // Làm trống giỏ hàng
        public void Clear()
        {
            items.Clear();
        }

        // Cập nhật số lượng của sản phẩm đã chọn
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);

            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}
