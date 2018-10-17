using Sitecore.Commerce.Core;
using Sitecore.Commerce.Engine;
using Sitecore.Commerce.Engine.Connect.Pipelines.Carts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;

using System;
using System.Linq;

using Sitecore.Commerce.Engine.Connect.Entities;
using Plugin.Payment.Heidelpay.Components;
using Heidelpay.Connect.Entities;

namespace Heidelpay.Connect.Pipelines.AddPaymentInfoToCart
{
    public class AddHeidelpayPaymentInfoToCart : CartProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
            Assert.IsNotNull(args, "args");
            Assert.IsNotNull(args.Request, "args.Request");
            Assert.IsNotNull(args.Result, "args.Result");
            Assert.IsTrue(args.Request is AddPaymentInfoRequest, "args.Request is AddPaymentInfoRequest");
            Assert.IsTrue(args.Result is AddPaymentInfoResult, "args.Result is AddPaymentInfoResult");
            var request = (AddPaymentInfoRequest)args.Request;
            var result = (AddPaymentInfoResult)args.Result;
            
            Assert.IsNotNull(request.Cart, "request.Cart");
            Assert.IsNotNull(request.Payments, "request.Payments");
            Cart cart = request.Cart;

            Container container = GetContainer(cart.ShopName, cart.UserId, cart.CustomerId, "", args.Request.CurrencyCode, new DateTime?());

            var heidelpayPayments = request.Payments.OfType<HeidelpayPaymentInfo>();

            if (!heidelpayPayments.Any())
            {
                return;
            }

            foreach(var heidelpayPayment in heidelpayPayments)
            {

                var ogonePaymentComponent = new HeidelpayPaymentComponent
                {
                    Id = Guid.NewGuid().ToString("N"),
                    PaymentMethod = new EntityReference()
                    {
                        EntityTarget = heidelpayPayment.PaymentMethodID
                    },
                    Amount = Money.CreateMoney(heidelpayPayment.Amount)
                };
                    
                var command = Proxy.DoCommand(container.AddHeidelpayPayment(cart.ExternalId, ogonePaymentComponent));
            }

            Sitecore.Commerce.Plugin.Carts.Cart cart1 = GetCart(cart.UserId, cart.ShopName, cart.ExternalId, cart.CustomerId, args.Request.CurrencyCode);
            if (cart1 != null)
            {
                result.Cart = TranslateCartToEntity(cart1, result);
                result.Payments = result.Cart.Payment.ToList();
            }

            // Abort the rest of the pipeline. Because the default commerce engine connect processors will fail
            // if there is a payment other than creditcard or giftcard.
            args.AbortPipeline();
        }
    }
}