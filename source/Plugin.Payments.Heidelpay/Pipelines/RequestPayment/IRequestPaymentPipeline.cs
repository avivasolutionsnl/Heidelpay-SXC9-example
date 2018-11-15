using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Payment.Heidelpay.Pipelines.RequestPayment
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Pipelines:IRequestPaymentPipeline")]
    public interface IRequestPaymentPipeline : IPipeline<RequestPaymentArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock<RequestPaymentArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }

}
