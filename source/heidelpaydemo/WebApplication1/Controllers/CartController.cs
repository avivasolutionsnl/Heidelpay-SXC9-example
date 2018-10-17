using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using System;
using System.Web.Mvc;
using Sitecore.Analytics;
using System.Linq;
using System.Collections.Generic;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Services.Payments;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        CartServiceProvider csp = new CartServiceProvider();

        private Cart Cart => csp.LoadCart(new LoadCartRequest("CommerceEngineDefaultStorefront", "default", ContactId))?.Cart;

        private const string StaticVisitorId = "{74E29FDC-8523-4C4F-B422-23BBFF0A342A}";


        private static string ContactId
        {
            get
            {
                if (Tracker.Current != null && Tracker.Current.IsActive && Tracker.Current.Contact != null)
                {
                    return Tracker.Current.Contact.ContactId.ToString("D");
                }

                // Generate our own tracking id if needed for the experience editor.
                if (global::Sitecore.Context.PageMode.IsExperienceEditor)
                {
                    return StaticVisitorId;
                }

                throw new InvalidOperationException("Tracking not enabled.");
            }
        }

        // GET: Cart
        public ActionResult Index()
        {
            return View(Cart);
        }

        public ActionResult Add()
        {
            var request = new AddCartLinesRequest(Cart, new[]{
                new CommerceCartLine
            {
                Product = new CommerceCartProduct
                {
                    ProductCatalog = "Habitat_Master",
                    ProductId = "6042185",
                    ProductVariantId = "56042185"
                },
                Quantity = 1
            }});

            var result = csp.AddCartLines(request);

            if (!result.Success) {
                throw new InvalidOperationException(string.Join(",", result.SystemMessages.Select(x => x.Message)));
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult AddShippingInfo()
        {
            string partyId = Guid.NewGuid().ToString();
            var cart = Cart;
            cart.Parties = new List<Sitecore.Commerce.Entities.Party>{new CommerceParty
            {
                ExternalId = partyId,
                Name = "Shipping",
                FirstName = "John",
                LastName = "West",
                Address1 = "Hollywood Boulevard 2343",
                City = "Los Angeles",
                ZipPostalCode = "344444",
                CountryCode = "US",
                Country = "United States",
                State = "CA"
            }};
            
            var request = new Sitecore.Commerce.Engine.Connect.Services.Carts.AddShippingInfoRequest(cart, new List<ShippingInfo>
            {
                new CommerceShippingInfo
                {
                    ShippingOptionType = ShippingOptionType.ShipToAddress,
                    ShippingMethodID = "cf0af82a-e1b8-45c2-91db-7b9847af287c",
                    ShippingMethodName = "Standard",
                    PartyID = partyId
                }
            }, ShippingOptionType.ShipToAddress);

            var result = csp.AddShippingInfo(request);

            if (!result.Success)
            {
                throw new InvalidOperationException(string.Join(",", result.SystemMessages.Select(x => x.Message)));
            }

            return RedirectToAction(nameof(Index));
        }

        
    }
}