using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Payment.Heidelpay.Components;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;


namespace Plugin.Payment.Heidelpay.Pipelines.HandleResponse.Blocks
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Blocks:HandleResponseBlock")]
    public class HandleResponseBlock : PipelineBlock<HandleResponseArgument, bool, CommercePipelineExecutionContext>
    {
        private readonly IGetOrderPipeline getOrderPipeline;
        private readonly IPersistEntityPipeline persistEntityPipeline;

        public HandleResponseBlock(IGetOrderPipeline getOrderPipeline, IPersistEntityPipeline persistEntityPipeline)
        {
            this.getOrderPipeline = getOrderPipeline;
            this.persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<bool> Run(HandleResponseArgument arg, CommercePipelineExecutionContext context)
        {
            var orderId = arg.Parameters["IDENTIFICATION.TRANSACTIONID"];

            // Fulfill pre-condition: get cart
            Order order = await getOrderPipeline.Run(orderId, context).ConfigureAwait(false);

            if (order == null)
            {
                await context.CommerceContext.AddMessage(context.CommerceContext.GetPolicy<KnownResultCodes>().Error,
                                                "EntityNotFound",
                                                new object[] { orderId },
                                                $"Entity {0} was not found.");

                return false;
            }

            if (!order.HasComponent<HeidelpayPaymentComponent>())
            {
                return false;
            }

            var payment = order.GetComponent<HeidelpayPaymentComponent>();
            UpdatePayment(arg.Parameters, payment);

            await persistEntityPipeline.Run(new PersistEntityArgument(order), context);

            return true;
        }

        private static void UpdatePayment(Dictionary<string, string> parameters, HeidelpayPaymentComponent payment)
        {
            var status = parameters["PROCESSING.RESULT"];
            var timestamp = parameters["PROCESSING.TIMESTAMP"];

            payment.IsSettled = string.Compare("ACK", status, true) == 0;

            DateTime settledAt;
            if (DateTime.TryParse(timestamp, out settledAt))
            {
                payment.SettledAt = settledAt;
            }
        }
    }
}
