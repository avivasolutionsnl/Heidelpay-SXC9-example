using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;


namespace Plugin.Payment.Heidelpay.Pipelines.HandleResponse.Blocks
{
    [PipelineDisplayName("Plugin.Payment.Heidelpay:Blocks:ValidateResponseBlock")]
    public class ValidateResponseBlock : PipelineBlock<HandleResponseArgument, HandleResponseArgument, CommercePipelineExecutionContext>
    {
        public override Task<HandleResponseArgument> Run(HandleResponseArgument arg, CommercePipelineExecutionContext context)
        {
            var parameterValues = string.Join(", ", arg.Parameters.Select(p => $"{p.Key}: {p.Value}"));

            context.Logger.LogInformation($"Received response from Heidelpay, parameters: {parameterValues}");
            
            // todo
            return Task.FromResult(arg);
        }
    }
}
