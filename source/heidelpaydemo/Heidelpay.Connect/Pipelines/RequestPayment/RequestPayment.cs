using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Diagnostics;
using System;

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

            var redirect = Proxy.GetValue(GetContainer(request.GetShopName(), request.CustomerId, "", "", "", new DateTime?())
                .RequestPayment(request.OrderId));

            result.RedirectUrl = redirect;
        }
    }
}