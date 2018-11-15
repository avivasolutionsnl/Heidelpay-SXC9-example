using Plugin.Payment.Heidelpay.Pipelines.RequestPayment;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Orders;
using System;
using System.Threading.Tasks;

namespace Plugin.Payment.Heidelpay.Commands
{
    public class RequestPaymentCommand : CommerceCommand
    {
        private readonly IRequestPaymentPipeline requestPaymentPipeline;
        private readonly IGetOrderPipeline getOrderPipeline;

        public RequestPaymentCommand(IGetOrderPipeline getOrderPipeline, IRequestPaymentPipeline requestPaymentPipeline, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.getOrderPipeline = getOrderPipeline;
            this.requestPaymentPipeline = requestPaymentPipeline;
        }

        public virtual async Task<bool> Process(CommerceContext commerceContext, string orderId)
        {
            // Run the pipeline
            using (CommandActivity.Start(commerceContext, this))
            {
                bool result = false;

                await PerformTransaction(commerceContext, (async () =>
                {
                    var requestPaymentArgument = new RequestPaymentArgument(orderId);

                    CommercePipelineExecutionContextOptions pipelineContextOptions = commerceContext.GetPipelineContextOptions();

                    result = await requestPaymentPipeline.Run(requestPaymentArgument, pipelineContextOptions).ConfigureAwait(false);
                }));

                    return result;
                }
        }
    }
}
