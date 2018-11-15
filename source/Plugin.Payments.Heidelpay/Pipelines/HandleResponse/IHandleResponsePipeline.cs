using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Plugin.Payment.Heidelpay.Pipelines.HandleResponse
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Pipelines:IHandleResponsePipeline")]
    public interface IHandleResponsePipeline : IPipeline<HandleResponseArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock<HandleResponseArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }

}
