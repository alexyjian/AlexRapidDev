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
            if (admin.IsLocked)
            {
                ViewBag.ErrorMessage = "用户被锁定";
                return View();
            }

            //更新登录时间
            admin.LastLoginDateTime=DateTime.Now;
            await context.EditAsync(admin);

            //生成登录Token
            ALEXFWAuthentication.SignIn(admin.Index.ToString(), false);
            Session["AdminLogin"] = admin;
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignOut()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("SignIn");
            ALEXFWAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [EntityAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        [EntityAuthorize]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string Password, string NewPassword, string ConfirmationPassword)
        {
            var result = new ChangePasswordModel();
            if (string.IsNullOrEmpty(Password))
            {
                result.IsSuc = false;
                result.responseText = "原密码不能为空";
                return Json(result);
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                result.IsSuc = false;
                result.responseText = "新密码不能为空";
                return Json(result);
            }

            if (NewPassword != ConfirmationPassword)
            {
                result.IsSuc = false;
                result.responseText = "两次输入的密码不一样";
                return Json(result);
            }
            var strid = System.Web.HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(strid))
            {
                result.IsSuc = false;
                result.responseText = "登录超时请重新登录";
                return Json(result);
            }
            var id = Guid.Parse(strid);
            var UserContext = EntityBuilder.GetContext<Entity.UserAndRole.Admin>();
            var CurrentUser = await UserContext.GetEntityAsync(id);
            if (!CurrentUser.VerifyPassword(Password))
            {
                result.IsSuc = false;
                result.responseText = "原密码输入错误";
                return Json(result);
            }
            //设置新密码
            CurrentUser.SetPassword(ConfirmationPassword);
            //持久化
            UserContext.Edit(CurrentUser);

            //跳转到重新登录
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("SignIn");

            //旧凭据注销
            ALEXFWAuthentication.SignOut();

            result.IsSuc = true;
            result.responseText = "修改成功请重新登录";
            return Json(result);
        }
    }
}

/// <summary>
/// 修改密码状态实体
/// </summary>
public class ChangePasswordModel
{
    public bool IsSuc { get; set; }
    public string responseText { get; set; }
}