using Sitecore.Commerce.Core;

namespace Plugin.Payment.Heidelpay.Pipelines.RequestPayment
{
    public class RequestPaymentArgument : PipelineArgument
    {
        public RequestPaymentArgument(string orderId)
        {
            OrderId = orderId;
        }

        public string OrderId { get; set; }
    }

}
