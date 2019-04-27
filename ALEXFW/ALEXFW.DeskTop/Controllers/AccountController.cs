using System;
using System.Configuration;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ALEXFW.CommonUtility;
using ALEXFW.Entity.Members;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop.Controllers
{
    public class AccountController : EntityController
    {
        public AccountController(IEntityContextBuilder builder) : base(builder)
        {
        }

        /// <summary>
        ///     登录
        /// </summary>
        /// <param name="returnUrl">跳转地址</param>
        /// <returns></returns>
        public ActionResult SignIn(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                //默认跳转首页
                if (returnUrl == null)
                    return RedirectToAction("Index", "Home");
                else
                    return Redirect(returnUrl);

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.UrlReferrer == null ? "/Home/Index" : Request.UrlReferrer.PathAndQuery;

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        /// <summary>
        ///     登录提交处理
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SignIn(string username, string password, bool rememberMe = false)
        {
            var context = EntityBuilder.GetContext<Member>();
            var member = await context.Query().SingleOrDefaultAsync(t => t.Username.ToLower() == username.ToLower());

            if (member == null)
            {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
                return Content("账号不存在！");
            }

            if (!member.VerifyPassword(password))
            {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
                return Content("密码错误！");
            }

            if (!member.IsEnabled)
            {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
                return Content("用户被禁用！");
            }
            //更新最近登录时间
            member.LastLoginDateTime = DateTime.Now;
            await context.EditAsync(member);

            //登录操作
            ALEXFWAuthentication.SignIn(member.Index.ToString(), rememberMe);
            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        ///     注销
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("SignIn");
            ALEXFWAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        public ActionResult My()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }
    }
}