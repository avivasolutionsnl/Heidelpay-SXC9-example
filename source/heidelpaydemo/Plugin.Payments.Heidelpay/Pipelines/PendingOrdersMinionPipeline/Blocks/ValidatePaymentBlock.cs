using Plugin.Payment.Heidelpay.Components;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;

using System.Threading.Tasks;

namespace Plugin.Payment.Heidelpay.Pipelines.PendingOrdersMinionPipeline.Blocks
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Blocks:ValidatePaymentBlock")]
    public class ValidatePaymentBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        public override Task<Order> Run(Order arg, CommercePipelineExecutionContext context)
        {
            if (!arg.HasComponent<HeidelpayPaymentComponent>())
            {
                return Task.FromResult(arg);
            }

            var payment = arg.GetComponent<HeidelpayPaymentComponent>();

            if (!payment.IsSettled)
            {
                context.Abort("Heidelpay payment has not been received", context);
            }

            return Task.FromResult(arg);
        }
    }
}
