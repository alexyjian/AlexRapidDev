using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALEXFW.DeskTop.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var route = context.Routes.MapRoute<Entity.UserAndRole.Admin>(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "ALEXFW.DeskTop.Areas.Admin.Controllers" }
                );
            route.DataTokens["authArea"] = "Admin";
            route.DataTokens["area"] = "Admin";
            route.DataTokens["loginUrl"] = "/Admin/Account/SignIn";
        }
    }
}