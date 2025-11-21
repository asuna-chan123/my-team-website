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
    public class Carts
    {
        private List<CartsItem> items = new List<CartsItem>();

        public IEnumerable<CartsItem> Items => items;

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Danh sách các sản phẩm cùng danh mục với các sản phẩm trong giỏ
        public PagedList.IPagedList<Product> SimilarProducts { get; set; }

        // Grouped items by category (computed property)
        public List<IGrouping<string, CartsItem>> GroupedItems => items.GroupBy(i => i.Category).ToList();

        // Thêm sản phẩm vào giỏ
        public void AddItem(int productId, string productImage, string productName,
                           decimal unitPrice, int quantity, string category)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == productId);

            if (existingItem == null)
            {
                items.Add(new CartsItem
                {
                    ProductID = productId,
                    ImagePro = productImage,
                    NamePro = productName,
                    UnitPrice = unitPrice,
                    Quantity = quantity,
                    Category = category
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
