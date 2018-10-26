using Plugin.Payment.Heidelpay.Models;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Diagnostics;
using System;
using System.Linq;

namespace Heidelpay.Connect.Pipelines.RequestPayment
{
    public class RequestPayment : PipelineProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            Assert.IsTrue(args.Request is RequestPaymentRequest, "args.Request is UpdatePaymentStatusRequest");
            Assert.IsTrue(args.Result is RequestPaymentResult, "args.Result is UpdatePaymentStatusResult");

            var request = (RequestPaymentRequest)args.Request;
            var result = (RequestPaymentResult)args.Result;

            var command = Proxy.DoCommand<CommerceCommand>(GetContainer(request.GetShopName(), request.CustomerId, "", "", "", new DateTime?())
                .RequestPayment(request.OrderId));

            PaymentRequested model = command.Models.OfType<PaymentRequested>().SingleOrDefault();

            if(model == null)
            {
                // Todo: Error handling
                return;
            }

            result.RedirectUrl = model.RedirectUrl;
        }
    }
}