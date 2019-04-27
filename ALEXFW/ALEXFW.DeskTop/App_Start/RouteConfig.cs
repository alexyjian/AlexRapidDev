using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ALEXFW.Entity.Members;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute<Member>(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                namespaces: new string[] { "ALEXFW.DeskTop.Controllers" }
            );
        }
    }
}
