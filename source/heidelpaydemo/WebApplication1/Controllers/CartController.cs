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

namespace website.Controllers
{
    public class CartController : CommerceController
    {

        // GET: Cart
        public ActionResult Index()
        {
            IEnumerable<CommerceSellableItemSearchResultItem> sellableItems;

            try
            {
                var index = new CommerceSearchManager().GetIndex();

                using (var searchContext = index.CreateSearchContext())
                {
                    sellableItems = searchContext.GetQueryable<CommerceSellableItemSearchResultItem>()
                        .Where(x => x.CommerceSearchItemType == CommerceSearchItemType.SellableItem)
                        .ToList();
                }
            }
            catch
            {
                sellableItems = new List<CommerceSellableItemSearchResultItem>
                {
                    new CommerceSellableItemSearchResultItem
                    {
                        DisplayName = "Vertus 6 1 2\" 3-Way Floorstanding Speaker",
                        ItemId = new ID("{7B82EDB1-9D0F-6CF3-046F-4E90D69C4259}")
                    }
                };
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
    }
}