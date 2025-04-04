using System.Web.Mvc;

namespace laudosPiscinasProject.Api.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Resolve(string aspxerrorpath)
        {
            return this.Redirect("~/#/" + aspxerrorpath);
        }
    }
}