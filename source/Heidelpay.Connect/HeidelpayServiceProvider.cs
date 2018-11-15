using Heidelpay.Connect.Pipelines.HandleResponse;
using Heidelpay.Connect.Pipelines.RequestPayment;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Services;
using System.Collections.Generic;

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

        public void HandleResponse(string shopName, Dictionary<string, string> parameters)
        {
            var request = new HandleResponseRequest(parameters);
            request.SetShopName(shopName);
            
            RunPipeline<HandleResponseRequest, HandleResponseResult>("heidelpay.handleResponse", request);
        }
    }
}
