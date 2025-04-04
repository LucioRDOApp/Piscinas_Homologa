using System.Web.Mvc;

namespace laudosPiscinasProject.Api.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.ActionDescriptor.ActionName.ToLower() == "entrar" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "login")
            {
                filterContext.Result = RedirectToAction("Index", "Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}