using Sitecore.Commerce.Services;
using System.Collections.Generic;

namespace Heidelpay.Connect.Pipelines.HandleResponse
{
    public class HandleResponseRequest : ServiceProviderRequest
    {
        public HandleResponseRequest(Dictionary<string, string> parameters)
        {
            Parameters = parameters;
        }

        public Dictionary<string, string> Parameters { get; set; }
    }
}