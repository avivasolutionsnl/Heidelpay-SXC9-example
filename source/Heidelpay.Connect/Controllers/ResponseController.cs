using System.Linq;
using System.Web.Mvc;

namespace Heidelpay.Connect.Controllers
{
    public class ResponseController : Controller
    {
        [HttpPost]
        public ActionResult Handle(FormCollection parameters)
        {
            var dictionary = parameters.AllKeys.ToDictionary(c => c, y => parameters[y]);

            new HeidelpayServiceProvider().HandleResponse("CommerceEngineDefaultStorefront", dictionary);

            return Content("http://sitecore/success");
        }
    }
}