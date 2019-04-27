using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ALEXFW.CommonUtility;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class MyDepartmentController : EntityController
    {
        public MyDepartmentController(IEntityContextBuilder builder) : base(builder)
        {
        }

        // GET: Admin/MDepartment
        public ActionResult Index()
        {
            Entity.UserAndRole.Admin admin = User.GetUser<Entity.UserAndRole.Admin>();
            var department = admin.Department;
            return View(department);
        }

        [HttpPost]
        [ValidateInput(false)]  
        public async  Task<ActionResult> Index(Department model, HttpPostedFileBase Logo)
        {
            var context = EntityBuilder.GetContext<Department>();
            if (model.DepartmentName.Length > 15||string.IsNullOrEmpty(model.DepartmentName))
            {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
                return Content("名称不能为空或不能超过15个字符！");
            }

            var savePath="";
            if (Logo != null)
            {
                //上传店铺Logo
                if (Logo.ContentLength > 0)
                {
                     var extension = Path.GetExtension(Logo.FileName);
                    //获得保存路径
                    string filePath = Path.Combine(HttpContext.Server.MapPath("../WebUploadFile/Department/"),model.Index.ToString()+extension);
                    Logo.SaveAs(filePath);
                    savePath = "/WebUploadFile/Department/" + model.Index + extension;
                    model.Logo = savePath;
                }
            }
            
            var department = await context.GetEntityAsync(model.Index);
            department.DepartmentName = model.DepartmentName;
            department.DSCN = Request.Form["DSCN"];
            if (!string.IsNullOrEmpty(savePath)&&department.Logo != savePath)
                department.Logo = model.Logo;
            await context.EditAsync(department);
            return new HttpStatusCodeResult(200);
        }

    }
}