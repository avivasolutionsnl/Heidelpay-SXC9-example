using Newtonsoft.Json;
using Plugin.Payment.Heidelpay.Models;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Plugin.Payment.Heidelpay.Pipelines.RequestPayment.Blocks
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Blocks:RequestPaymentBlock")]
    public class RequestPaymentBlock : PipelineBlock<RequestPaymentArgument, bool, CommercePipelineExecutionContext>
    {
        private IGetOrderPipeline getOrderPipeline;
        private IPersistEntityPipeline persistEntityPipeline;
        private readonly IFindEntitiesInListPipeline findEntitiesInListPipeline;

        public RequestPaymentBlock(IGetOrderPipeline getOrderPipeline, IPersistEntityPipeline persistEntityPipeline, IFindEntitiesInListPipeline findEntitiesInListPipeline)
        {
            this.getOrderPipeline = getOrderPipeline;
            this.persistEntityPipeline = persistEntityPipeline;
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

            var request = BuildRequest(order);

            string redirectUrl = await Post(request);

            context.CommerceContext.AddModel(new PaymentRequested(redirectUrl));

            return true;
        }

        private Dictionary<string, string> BuildRequest(Order order)
        {
            return new Dictionary<string, string>
            {
                {"REQUEST.VERSION", "1.0"},
                {"TRANSACTION.CHANNEL", "31HA07BC8142C5A171749A60D979B6E4" },
                {"IDENTIFICATION.TRANSACTIONID", order.OrderConfirmationId },
                {"TRANSACTION.MODE", "INTEGRATOR_TEST" },
                {"PRESENTATION.AMOUNT", order.Totals.GrandTotal.Amount.ToString() },
                {"PRESENTATION.CURRENCY", "EUR" },
                {"PAYMENT.CODE", "CC.DB" },
                {"SECURITY.SENDER", "31HA07BC8142C5A171745D00AD63D182" },
                {"USER.LOGIN", "31ha07bc8142c5a171744e5aef11ffd3" },
                {"USER.PWD", "93167DE7" },
                {"FRONTEND.ENABLED", "true" },
                {"FRONTEND.RESPONSE_URL", "http://www.merchantshop.com/paymentResult?jsessionid=12343215413243214213" }
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
