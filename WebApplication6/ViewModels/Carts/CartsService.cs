using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.ViewModels.CartViewModels
{
    public class CartsService
    {
        private readonly HttpSessionStateBase session;
        private const string CartSessionKey = "Cart";

        public CartsService(HttpSessionStateBase session)
        {
            this.session = session;
        }

        public Carts GetCart()
        {
            var cart = (Carts)session["Cart"];

            if (cart == null)
            {
                cart = new Carts();
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
