using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plugin.Payment.Heidelpay.Commands;
using Plugin.Payment.Heidelpay.Components;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Payments;

namespace Plugin.Payment.Heidelpay.Controllers
{
    public class CommandsController : CommerceController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
              : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPut]
        [Route("AddHeidelpayPayment()")]
        public async Task<IActionResult> AddHeidelpayPayment([FromBody] ODataActionParameters value)
        {
            CommandsController commandsController = this;
            if (!commandsController.ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(commandsController.ModelState);
            }

            if (!value.ContainsKey("cartId") || !value.ContainsKey("payment"))
            {
                return new BadRequestObjectResult(value);
            }

            string cartId = value["cartId"].ToString();
            string payment = value["payment"].ToString();

            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(payment))
            {
                return new BadRequestObjectResult(value);
            }

            HeidelpayPaymentComponent paymentComponent = JsonConvert.DeserializeObject<HeidelpayPaymentComponent>(payment);
            AddPaymentsCommand command = commandsController.Command<AddPaymentsCommand>();

            Cart cart = await command.Process(commandsController.CurrentContext, cartId, new List<PaymentComponent>() {
                paymentComponent
            });

            return new ObjectResult(command);
        }

        [HttpPut]
        [Route("RequestPayment()")]
        public async Task<IActionResult> RequestPayment([FromBody] ODataActionParameters value)
        {
            if (!ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(ModelState);
            }

            string orderId = value["orderId"].ToString();


            RequestPaymentCommand command = Command<RequestPaymentCommand>();

            string result = await command.Process(CurrentContext, orderId);

            return new ObjectResult(result);
        }
    }
}
