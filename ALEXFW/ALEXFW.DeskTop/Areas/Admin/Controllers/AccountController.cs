using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ALEXFW.DataAccess;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class AccountController : EntityController
    {
        public AccountController(IEntityContextBuilder builder) : base(builder)
        {
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(string username, string password)
        {
            var context = EntityBuilder.GetContext<Entity.UserAndRole.Admin>();
            var admin = await context.Query().SingleOrDefaultAsync(t => t.Username.ToLower() == username.ToLower());
            if (admin == null)
            {
                ViewBag.ErrorMessage = "管理员不存在";
                return View();
            }
            if (!admin.VerifyPassword(password))
            {
                ViewBag.ErrorMessage = "密码错误";
                return View();
            }
            ALEXFWAuthentication.SignIn(admin.Index.ToString(), false);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignOut()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("SignIn");
            ALEXFWAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }
    }
}