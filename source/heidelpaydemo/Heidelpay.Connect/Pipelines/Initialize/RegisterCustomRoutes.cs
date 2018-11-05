using Sitecore.Mvc.Pipelines.Loader;
using Sitecore.Pipelines;
using System.Web.Mvc;
using System.Web.Routing;

namespace Heidelpay.Connect.Pipelines.Initialize
{
    public class RegisterCustomRoutes : InitializeRoutes
    {
        public override void Process(PipelineArgs args)
        {
            //register MVC/AJAX route(s)
            RouteTable.Routes.MapRoute("HeidelPayResponseHandler", "Heidelpay/Response", new { controller = "Response", action = "Handle" });
        }
    }
}