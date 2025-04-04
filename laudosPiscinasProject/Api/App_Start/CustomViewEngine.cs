using System.Collections.Generic;
using System.Web.Mvc;

namespace laudosPiscinasProject.Api.App_Start
{
    public class CustomViewEngine : RazorViewEngine
    {
        public CustomViewEngine() : base()
        {
            string[] ViewLocations = new string[] {
            "~/Api/Views/{1}/{0}.cshtml",
            "~/Api/Views/{1}/{0}.vbhtml",
            "~/Api/Views/Shared/{0}.cshtml",
            "~/Api/Views/Shared/{0}.vbhtml"
            };
            AreaViewLocationFormats = ViewLocations;
            AreaMasterLocationFormats = ViewLocations;
            AreaPartialViewLocationFormats = ViewLocations;
            ViewLocationFormats = ViewLocations;
            MasterLocationFormats = ViewLocations;
            PartialViewLocationFormats = ViewLocations;
        }

        private string[] AppendLocationFormats(string[] newLocations, string[] defaultLocations)
        {
            List<string> viewLocations = new List<string>();
            viewLocations.AddRange(newLocations);
            viewLocations.AddRange(defaultLocations);
            return viewLocations.ToArray();
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return base.CreatePartialView(controllerContext, partialPath);
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return base.FileExists(controllerContext, virtualPath);
        }
    }
}
