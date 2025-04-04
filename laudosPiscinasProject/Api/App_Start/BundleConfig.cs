using System.Web.Optimization;

namespace laudosPiscinasProject.Api.App_Start
{
    public class BundleConfig
    {

        //                        "~/Assets/angular/Scripts/jquery-3.2.1.min.js",
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                        "~/Assets/angular/Scripts/jquery-{version}.js",
                        "~/Assets/Scripts/lib/arrive.js",
                        "~/Assets/Scripts/lib/jquery-sortable-with-depth.js",
                        "~/Assets/Scripts/lib/multiselect.js",
                        "~/Assets/Scripts/lib/jquery.scrollbar.js",
                        "~/Assets/Scripts/lib/jquery.dataTables.min.js",
                        "~/Assets/angular/Scripts/toastr.min.js",
                        "~/Assets/angular/Scripts/custom.js",
                        "~/Assets/Scripts/lib/jquery.validate.min.js",
                        "~/Assets/angular/Scripts/angular.js",
                        "~/Assets/angular/Scripts/angular-resource.js",
                        "~/Assets/angular/Scripts/angular-router.js",
                        "~/Assets/angular/Scripts/angular-ui-router.js",
                        "~/Assets/angular/Scripts/angular-ui-mask.js",
                        "~/Assets/angular/Scripts/angular-base64-upload.js",
                        "~/Assets/angular/Scripts/angular-animate.min.js",
                        "~/Assets/angular/Scripts/angular-aria.min.js",
                        "~/Assets/angular/Scripts/angular-material.min.js",
                        "~/Assets/angular/Scripts/jquery.maskMoney.min.js",
                        "~/Assets/angular/Scripts/jquery.inputmask.bundle.js",
                        //"~/Assets/angular/Scripts/angular-locale_pt-br.js",
                        "~/Client/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/controllers").IncludeDirectory(
                        "~/Client/Controllers", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Assets/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Assets/Scripts/bootstrap/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //            "~/Assets/angular/Style/bootstrap.css",
            //            "~/Assets/angular/Style/toastr.css",
            //            "~/Assets/angular/Style/site.css",
            //            "~/Assets/angular/Style/custom.css"));


            bundles.Add(new StyleBundle("~/Styles").Include(

                        "~/Assets/Styles/bootstrap.min.css",
                        "~/Assets/Styles/bootstrap-no-spacing.css",
                        "~/Assets/Styles/material-kit.css",
                        "~/Assets/Styles/datepicker.css",
                        "~/Assets/Styles/fonts.css",
                        "~/Assets/angular/Style/toastr.css",
                        "~/Assets/Style/jquery.scrollbar.css",
                        "~/Assets/Styles/sortable.css",
                        "~/Assets/angular/Style/angular-material.min.css",
                        "~/Assets/Styles/custom.css"));


            bundles.Add(new ScriptBundle("~/Scripts").Include(
                        "~/Assets/Scripts/lib/datepicker.js",
                        "~/Assets/Scripts/lib/datepicker.pt-BR.js",
                        "~/Assets/Scripts/lib/bootstrap.js",
                        "~/Assets/Scripts/lib/material-kit.js",
                        "~/Assets/Scripts/lib/material.min.js",
                        "~/Assets/Scripts/lib/highcharts.js",
                        "~/Assets/Scripts/lib/exporting.js",
                        "~/Assets/Scripts/lib/main.js",
                        "~/Assets/Scripts/lib/moment.min.js"));
        }
    }
}