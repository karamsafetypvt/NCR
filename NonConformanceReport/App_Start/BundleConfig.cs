using System.Web;
using System.Web.Optimization;

namespace NonConformanceReport
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                   "~/assets/js/jquery.min.js"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/assets/css").Include(
                      "~/assets/css/font-face.css",
                      "~/assets/vendor/font-awesome-4.7/css/font-awesome.min.css",
                      "~/assets/vendor/font-awesome-5/css/fontawesome-all.min.css",
                      "~/assets/vendor/mdi-font/css/material-design-iconic-font.min.css",
                      "~/assets/vendor/bootstrap-4.1/bootstrap.min.css",
                      "~/assets/vendor/animsition/animsition.min.css",
                      "~/assets/vendor/bootstrap-progressbar/bootstrap-progressbar-3.3.4.min.css",
                      "~/assets/vendor/wow/animate.css",
                      "~/assets/vendor/css-hamburgers/hamburgers.min.css",
                      "~/assets/vendor/slick/slick.css",
                      "~/assets/vendor/select2/select2.min.css",
                      "~/assets/vendor/perfect-scrollbar/perfect-scrollbar.css",
                      "~/assets/css/theme.css",
                      "~/assets/vendor/datatable/jquery.dataTables.css",
                      "~/assets/css/sweetalert.css"
                ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/assets/js/jquery-3.4.1.min.js",
                "~/assets/js/popper.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/slick.min.js",
                "~/assets/js/wow.min.js",
                "~/assets/js/animsition.min.js",
                "~/assets/js/bootstrap-progressbar.min.js",
                "~/assets/js/jquery.waypoints.min.js",
                "~/assets/js/jquery.counterup.min.js",
                "~/assets/js/circle-progress.min.js",
                "~/assets/js/perfect-scrollbar.js",
                "~/assets/js/Chart.bundle.min.js",
                "~/assets/js/select2.min.js",
                "~/assets/js/jquery.dataTables.min.js",
                "~/assets/js/sweetalert-dev.js",
                "~/assets/js/main.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrapvalidation").Include(
                 "~/Scripts/jquery-ui.min.js",
     "~/Scripts/jquery.validate.min.js",
     "~/Scripts/jquery.validate.unobtrusive.min.js"
                ));
        }
    }
}
