using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Plugin.Payment.Heidelpay.Pipelines.HandleResponse
{
    public class HandleResponseArgument : PipelineArgument
    {
        public HandleResponseArgument(Dictionary<string, string> parameters)
        {
            Parameters = parameters;
        }

        public Dictionary<string, string> Parameters { get; set; }
    }

}
