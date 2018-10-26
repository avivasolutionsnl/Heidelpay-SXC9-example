using Heidelpay.Connect.Pipelines.RequestPayment;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Heidelpay.Connect
{
    public class HeidelpayServiceProvider : ServiceProvider
    {
        public string RequestPayment(CommerceOrder order)
        {
            var request = new RequestPaymentRequest
            {
                CustomerId = order.CustomerId,
                OrderId = order.OrderID
            };

            request.SetShopName(order.ShopName);

            return RunPipeline<RequestPaymentRequest, RequestPaymentResult>("heidelpay.requestPayment", request).RedirectUrl;
        }
    }
}
