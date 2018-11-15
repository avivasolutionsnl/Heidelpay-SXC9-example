using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using System;
using System.Web.Mvc;
using Sitecore.Analytics;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.Shipping;
using System.Collections.Generic;
using System.Linq;

namespace website.Controllers
{
    public class CommerceController : Controller
    {
        protected CartServiceProvider csp = new CartServiceProvider();

        protected Cart Cart => GetCart();

        protected const string StaticVisitorId = "{74E29FDC-8523-4C4F-B422-23BBFF0A342A}";

        private Cart GetCart()
        {
            var cart = csp.LoadCart(new Sitecore.Commerce.Engine.Connect.Pipelines.Arguments.LoadCartByNameRequest("CommerceEngineDefaultStorefront", "default", ContactId))?.Cart;

            if(cart.Shipping == null || !cart.Shipping.Any())
            {
                AddShippingInfo(cart);
            }

            return cart;
        }

        private void AddShippingInfo(Cart cart)
        {
            string partyId = Guid.NewGuid().ToString();
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
        }

        protected static string ContactId
        {
            get
            {
                if (Tracker.Current == null)
                {
                    Tracker.StartTracking();
                }

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
    }
}