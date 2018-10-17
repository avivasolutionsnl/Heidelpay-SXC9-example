using System.Collections.Generic;
using System.Linq;
using Heidelpay.Connect.Entities;
using Plugin.Payment.Heidelpay.Components;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Commerce.Pipelines;
using Sitecore.Diagnostics;

namespace Heidelpay.Connect.Pipelines.TranslateCartToEntity
{
    public class TranslateHeidelpayPayment
    {
        public void Process(ServicePipelineArgs args)
        {
            Assert.IsNotNull(args, "args");
            Assert.IsNotNull(args.Request, "args.Request");
            Assert.IsNotNull(args.Result, "args.Result");
            Assert.IsTrue(args.Request is TranslateCartToEntityRequest, "args.Request is TranslateCartToEntityRequest");
            Assert.IsTrue(args.Result is TranslateCartToEntityResult, "args.Result is TranslateCartToEntityResult");
            var request = (TranslateCartToEntityRequest)args.Request;
            var result = (TranslateCartToEntityResult) args.Result;

            var cart = result.TranslatedEntity;
            var heidelpayPaymentComponents = request.TranslateSource.Components.OfType<HeidelpayPaymentComponent>().ToList();

            var paymentInfoList = new List<HeidelpayPaymentInfo>();

            heidelpayPaymentComponents.ForEach(component => {
                paymentInfoList.Add(new HeidelpayPaymentInfo
                {
                    ExternalId = component.Id,
                    PaymentMethodID = component.PaymentMethod.EntityTarget,
                    Amount = component.Amount?.Amount ?? decimal.Zero
                });
            });

            cart.Payment = result.TranslatedEntity.Payment.Union(paymentInfoList).ToList();
        }
    }
}