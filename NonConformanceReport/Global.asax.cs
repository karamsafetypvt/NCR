using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NonConformanceReport
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

        }
        //protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        //{
        //    HttpContext.Current.Response.Headers.Remove("X-Powered-By");
        //    HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
        //    HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
        //    HttpContext.Current.Response.Headers.Remove("Server");
            //HttpContext.Current.Response.Headers.Remove("Content-Disposition");
        //}
        public static string getControllerNameFromUrl()
        {
            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
            RouteData rd = RouteTable.Routes.GetRouteData(context);
            String _controller = String.Empty;
            if (rd != null)
            {
                _controller = rd.GetRequiredString("controller");
                _controller += "/" + rd.GetRequiredString("action");

                //_controller += "/" + rd.GetRequiredString("action");
            }
            return _controller;
        }
    }
}
