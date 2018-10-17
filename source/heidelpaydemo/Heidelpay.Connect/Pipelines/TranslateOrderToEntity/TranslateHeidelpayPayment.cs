using Heidelpay.Connect.Entities;
using Plugin.Payment.Heidelpay.Components;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Heidelpay.Connect.Pipelines.TranslateOrderToEntity
{
    public class TranslateHeidelpayPayment 
    {
        public void Process(ServicePipelineArgs args)
        {
            Assert.IsNotNull(args, "args");
            Assert.IsNotNull(args.Request, "args.Request");
            Assert.IsNotNull(args.Result, "args.Result");
            Assert.IsTrue(args.Request is TranslateOrderToEntityRequest, "args.Request is TranslateOrderToEntityRequest");
            Assert.IsTrue(args.Result is TranslateOrderToEntityResult, "args.Result is TranslateOrderToEntityResult");
            var request = (TranslateOrderToEntityRequest)args.Request;
            var result = (TranslateOrderToEntityResult)args.Result;

            var heidelpayPaymentComponents = request.TranslateSource.Components.OfType<HeidelpayPaymentComponent>();

            List<PaymentInfo> paymentInfoList = new List<PaymentInfo>();

            foreach(var component in heidelpayPaymentComponents)
            {
                paymentInfoList.Add(new HeidelpayPaymentInfo
                {
                    ExternalId = component.Id,
                    PaymentMethodID = component.PaymentMethod.EntityTarget,
                    Amount = component.Amount?.Amount ?? decimal.Zero
                });
            }

            result.TranslatedEntity.Payment = result.TranslatedEntity.Payment.Union(paymentInfoList).ToList();
        }
    }
}