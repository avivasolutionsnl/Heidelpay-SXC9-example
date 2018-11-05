
using Plugin.Payment.Heidelpay.Models;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Diagnostics;
using System;
using System.Linq;

namespace Heidelpay.Connect.Pipelines.HandleResponse
{
    public class HandleResponse : PipelineProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            Assert.IsTrue(args.Request is HandleResponseRequest, "args.Request is HandleResponseRequest");
            Assert.IsTrue(args.Result is HandleResponseResult, "args.Result is HandleResponseResponse");

            var request = (HandleResponseRequest)args.Request;
            var result = (HandleResponseResult)args.Result;
            
            var command = Proxy.DoCommand(GetContainer(request.GetShopName(), "", "", "", "", new DateTime?())
                .HandleResponse(request.Parameters.Select(x => new Parameter { Key = x.Key, Value = x.Value}).ToList()));
        }
    }
}