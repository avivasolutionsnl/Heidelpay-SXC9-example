using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Plugin.Payment.Heidelpay.Pipelines.RequestPayment
{
    public class RequestPaymentPipeline : CommercePipeline<RequestPaymentArgument, bool>, IRequestPaymentPipeline
    {
        public RequestPaymentPipeline(IPipelineConfiguration<IRequestPaymentPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}
