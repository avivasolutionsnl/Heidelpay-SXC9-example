using Sitecore.Commerce.Services;

namespace Heidelpay.Connect.Pipelines.RequestPayment
{
    public class RequestPaymentRequest : ServiceProviderRequest
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }
    }
}