using Newtonsoft.Json;
using Plugin.Payment.Heidelpay.Models;
using Plugin.Payment.Heidelpay.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Plugin.Payment.Heidelpay.Pipelines.RequestPayment.Blocks
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Blocks:RequestPaymentBlock")]
    public class RequestPaymentBlock : PipelineBlock<RequestPaymentArgument, bool, CommercePipelineExecutionContext>
    {
        private IGetOrderPipeline getOrderPipeline;
        private readonly IFindEntitiesInListPipeline findEntitiesInListPipeline;

        public RequestPaymentBlock(IGetOrderPipeline getOrderPipeline, IFindEntitiesInListPipeline findEntitiesInListPipeline)
        {
            this.getOrderPipeline = getOrderPipeline;
            this.findEntitiesInListPipeline = findEntitiesInListPipeline;
        }

        public override async Task<bool> Run(RequestPaymentArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The argument cannot be null.");

            // Fulfill pre-condition: get cart
            Order order = await getOrderPipeline.Run(arg.OrderId, context).ConfigureAwait(false);

            if (order == null)
            {
                await context.CommerceContext.AddMessage(context.CommerceContext.GetPolicy<KnownResultCodes>().Error,
                                                "EntityNotFound",
                                                new object[] { arg.OrderId },
                                                $"Entity {0} was not found.");

                return false;
            }

            var request = BuildRequest(context.CommerceContext, order);

            string redirectUrl = await Post(request);

            context.CommerceContext.AddModel(new PaymentRequested(redirectUrl));

            return true;
        }

        private Dictionary<string, string> BuildRequest(CommerceContext context, Order order)
        {
            var policy = context.GetPolicy<HeidelpayPolicy>();

            return new Dictionary<string, string>
            {
                {"REQUEST.VERSION", "1.0"},
                {"TRANSACTION.CHANNEL", policy.TransactionChannel },
                {"IDENTIFICATION.TRANSACTIONID", order.OrderConfirmationId },
                {"TRANSACTION.MODE", policy.TransactionMode },
                {"PRESENTATION.AMOUNT", order.Totals.GrandTotal.Amount.ToString() },
                {"PRESENTATION.CURRENCY", "EUR" },
                {"PAYMENT.CODE", "CC.DB" },
                {"SECURITY.SENDER", policy.SecuritySender },
                {"USER.LOGIN", policy.UserLogin },
                {"USER.PWD", policy.UserPassword },
                {"FRONTEND.ENABLED", "true" },
                {"FRONTEND.RESPONSE_URL", policy.FrontendResponseUrl }
            };
        }

        private async Task<string> Post(Dictionary<string, string> parameters)
        {
            var client = new HttpClient();
            
            var content = new FormUrlEncodedContent(parameters);

            var response = await client.PostAsync("https://test-heidelpay.hpcgw.net/ngw/post", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
            }

            var body = await response.Content.ReadAsStringAsync();

            var values = System.Web.HttpUtility.ParseQueryString(body);

            if(values["PROCESSING.RESULT"] == "NOK")
            {
                throw new InvalidOperationException(values["PROCESSING.RETURN"]);
            }

            return values["FRONTEND.REDIRECT_URL"];
        }
    }
}
