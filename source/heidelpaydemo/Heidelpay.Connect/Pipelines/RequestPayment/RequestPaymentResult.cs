using Sitecore.Commerce.Services;

namespace Heidelpay.Connect.Pipelines.RequestPayment
{
    public class RequestPaymentResult : ServiceProviderResult
    {
        public string RedirectUrl { get; set; }
    }
}