using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Plugin.Payment.Heidelpay.Pipelines.HandleResponse
{
    public class HandleResponsePipeline : CommercePipeline<HandleResponseArgument, bool>, IHandleResponsePipeline
    {
        public HandleResponsePipeline(IPipelineConfiguration<IHandleResponsePipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}
