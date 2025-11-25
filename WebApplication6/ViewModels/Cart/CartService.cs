using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.ViewModels.CartViewModels
{
    public class CartService
    {
        private readonly HttpSessionStateBase session;
        private const string CartSessionKey = "Cart";

        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }

        public Cart GetCart()
        {
            var cart = (Cart)session["Cart"];

            if (cart == null)
            {
                cart = new Cart();
                session["Cart"] = cart;
            }

            return cart;
        }

        public void ClearCart()
        {
            session["Cart"] = null;
        }
    }
}
