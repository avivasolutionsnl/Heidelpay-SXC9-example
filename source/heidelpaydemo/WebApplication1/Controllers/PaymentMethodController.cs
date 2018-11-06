using Heidelpay.Connect.Entities;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using website.Models;

namespace WebApplication1.Controllers
{
    public class PaymentMethodController : CommerceController
    {
        // GET: PaymentMethod
        public ActionResult Index()
        {
            CommerceParty party = null;
            if (Cart.Payment != null && Cart.Payment.Any())
            {
                var paymentInfo = Cart.Payment.Single() as HeidelpayPaymentInfo;
                party = Cart.Parties.SingleOrDefault(x => x.ExternalId == paymentInfo.PartyID) as CommerceParty;
            }
                        
            return View(new PaymentMethodModel { Party = party });
        }

        public ActionResult Add(CommerceParty party)
        {
            // Always set external id to some magic number (Party in the loaded cart does not contain an ExternalId)
            // the AddPaymentInfoToCart processor looks for a ExternalId == PartyId
            party.ExternalId = Guid.NewGuid().ToString("D");

            var parties = new List<Sitecore.Commerce.Entities.Party> { party };

            var cart = Cart;
            cart.Parties = parties;

            var request = new AddPaymentInfoRequest(cart, new List<PaymentInfo>
            {
                new HeidelpayPaymentInfo
                {
                    Amount = Cart.Total.Amount,
                    PaymentMethodID = "c16ce432-b7e6-44ec-a9fc-01c4fc5caaca",
                    PartyID = party.ExternalId
                }
            });

            var result = csp.AddPaymentInfo(request);

            if (!result.Success)
            {
                throw new InvalidOperationException(string.Join(",", result.SystemMessages.Select(x => x.Message)));
            }

            return Redirect(LinkManager.GetItemUrl(Sitecore.Context.Item));
        }
    }
}