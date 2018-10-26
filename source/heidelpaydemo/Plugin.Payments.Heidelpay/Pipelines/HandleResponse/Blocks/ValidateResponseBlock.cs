using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;


namespace Plugin.Payment.Heidelpay.Pipelines.HandleResponse.Blocks
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Blocks:ValidateResponseBlock")]
    public class ValidateResponseBlock : PipelineBlock<HandleResponseArgument, HandleResponseArgument, CommercePipelineExecutionContext>
    {
        public override Task<HandleResponseArgument> Run(HandleResponseArgument arg, CommercePipelineExecutionContext context)
        {

            // todo
            return Task.FromResult(arg);
        }
    }
}
