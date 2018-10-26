using Plugin.Payment.Heidelpay.Pipelines.HandleResponse;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Payment.Heidelpay.Commands
{
    public class HandleResponseCommand : CommerceCommand
    {
        private readonly IHandleResponsePipeline handleResponsePipeline;
        private readonly IGetOrderPipeline getOrderPipeline;

        public HandleResponseCommand(IGetOrderPipeline getOrderPipeline, IHandleResponsePipeline handleResponsePipeline, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.getOrderPipeline = getOrderPipeline;
            this.handleResponsePipeline = handleResponsePipeline;
        }

        public virtual async Task<bool> Process(CommerceContext commerceContext, Dictionary<string, string> parameters)
        {
            // Run the pipeline
            using (CommandActivity.Start(commerceContext, this))
            {
                bool result = false;

                await PerformTransaction(commerceContext, (async () =>
                {
                    var handleResponseArgument = new HandleResponseArgument(parameters);

                    CommercePipelineExecutionContextOptions pipelineContextOptions = commerceContext.GetPipelineContextOptions();

                    result = await handleResponsePipeline.Run(handleResponseArgument, pipelineContextOptions).ConfigureAwait(false);
                }));

                return result;
            }
        }
    }
}
