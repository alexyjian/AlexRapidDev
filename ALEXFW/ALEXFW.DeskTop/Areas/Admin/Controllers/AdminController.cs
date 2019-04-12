using System;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ALEXFW.CommonUtility;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class AdminController : EntityController<Entity.UserAndRole.Admin>  
    {
        public AdminController(IEntityContextBuilder builder) : base(builder)
        {
        }
        public override async Task<ActionResult> Update(Guid id)
        {
            var deptContext = EntityBuilder.GetContext<Department>();
            var adminContext = EntityBuilder.GetContext<Entity.UserAndRole.Admin>();
            var departmentId = Request.Form["Department"].Trim().ToLower();
            var department =await deptContext.GetEntityAsync(Guid.Parse(departmentId));
            var group =(AdminGroup)Enum.Parse(typeof(AdminGroup), Request.Form["Group"]);
            

            ViewBag.ObjectID = id;
            ViewBag.ObjectType = department.DepartmentName;

            if (group.HasFlag(AdminGroup.店长)&&adminContext.Query().Any(x=>x.Department.Index==department.Index&&x.Group.HasFlag(AdminGroup.店长)))
            //判断只能有唯一店长
            {
                Response.StatusCode = 400;
                return new ContentResult
                {
                    Content = "该店铺已经有一名店长"
                };
            }

            return await Untils.GetUpdateAction(async (p, entity) =>
            {
                var result = await UpdateCore(entity);

                return result;
            }, UpdateProperty, id);
        }
    }
}