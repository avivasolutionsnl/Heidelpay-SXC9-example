using Heidelpay.Connect;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Services.Orders;
using System;
using System.Linq;
using System.Web.Mvc;

namespace website.Controllers
{
    public class ConfirmationController : CommerceController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Confirm()
        {
            var cart = Cart;
            cart.Email = "testme@hotmail.com";
            var result = new OrderServiceProvider().SubmitVisitorOrder(new SubmitVisitorOrderRequest(cart));

            if (!result.Success)
            {
                throw new InvalidOperationException(string.Join(",", result.SystemMessages.Select(x => x.Message)));
            }

            string redirectUrl = new HeidelpayServiceProvider().RequestPayment((CommerceOrder)result.Order);

            return Redirect(redirectUrl);
        }
    }
}