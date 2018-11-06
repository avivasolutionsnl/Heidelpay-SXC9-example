using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Services.Payments;
using Heidelpay.Connect.Entities;
using Sitecore.Commerce.Services.Orders;
using Heidelpay.Connect;
using Sitecore.Commerce.Engine.Connect.Search;
using website.Models;
using Sitecore.Links;
using Sitecore.Data;

namespace WebApplication1.Controllers
{
    public class CartController : CommerceController
    {

        // GET: Cart
        public ActionResult Index()
        {
            IEnumerable<CommerceSellableItemSearchResultItem> sellableItems;

            var index = new CommerceSearchManager().GetIndex();

            using (var searchContext = index.CreateSearchContext())
            {
                sellableItems = searchContext.GetQueryable<CommerceSellableItemSearchResultItem>()
                    .Where(x => x.CommerceSearchItemType == CommerceSearchItemType.SellableItem)
                    .ToList();
            }
            
            return View(new CartModel
            {
                Cart = Cart,
                SellableItems = sellableItems
            });
        }

        public ActionResult Add(string itemId)
        {
            var productItem = Database.GetDatabase("master").GetItem(new ID(itemId));

            var request = new AddCartLinesRequest(Cart, new[]{
                new CommerceCartLine
            {
                Product = new CommerceCartProduct
                {
                    ProductCatalog = "Habitat_Master",
                    ProductId = productItem.Name,
                    ProductVariantId = productItem.Children.FirstOrDefault()?.Name
                },
                Quantity = 1
            }});

            var result = csp.AddCartLines(request);

            if (!result.Success) {
                throw new InvalidOperationException(string.Join(",", result.SystemMessages.Select(x => x.Message)));
            }

            return Redirect(LinkManager.GetItemUrl(Sitecore.Context.Item));
        }
                

        public ActionResult ToOrder()
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