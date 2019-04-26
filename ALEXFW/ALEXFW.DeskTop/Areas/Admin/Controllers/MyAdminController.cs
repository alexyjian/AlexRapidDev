using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ALEXFW.CommonUtility;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class MyAdminController : EntityController<Entity.UserAndRole.Admin>
    {
        public MyAdminController(IEntityContextBuilder builder) : base(builder)
        {
        }

        [EntityAuthorize]
        public override async Task<ActionResult> Index(int page = 1, int size = 20, string parentpath = null,
            Guid? parentid = null, bool search = false)
        {
            return await List(page, size, "0", null);
        }

        [EntityAuthorize]
        public async Task<ActionResult> List(int page = 1, int size = 20, string adminGroup = "0",
            string nameOrCode = null)
        {
            var adminContext = EntityBuilder.GetContext<Entity.UserAndRole.Admin>();
            var admin = (Session["AdminLogin"] as Entity.UserAndRole.Admin);
            
            //按登录用户所在店铺查询所有该店铺管理者用户
            var lists = adminContext.Query()
                .Where(x =>
                    x.Department.Index == admin.Department.Index &&
                    !x.Group.HasFlag(AdminGroup.管理员)).OrderBy(x => x.EmployeeCode);

            //处理下拉
            switch (adminGroup)
            {
                case "0": //全部
                    adminGroup = "全部";
                    break;

                case "1": //未激活
                    lists = lists.Where(x => x.Group.HasFlag(AdminGroup.店长)).OrderBy(x => x.EmployeeCode);
                    adminGroup = "店长";
                    break;

                case "2": //已激活
                    lists = lists.Where(x => x.Group==AdminGroup.业务员).OrderBy(x => x.EmployeeCode);
                    adminGroup = "业务员";
                    break;

                default:
                    adminGroup = "全部";
                    break;
            }

            if (!string.IsNullOrEmpty(nameOrCode))
            {
                lists =
                    lists.Where(x => x.Username.Contains(nameOrCode) || x.EmployeeCode == nameOrCode)
                        .OrderBy(x => x.EmployeeCode);
            }

            // 分页
            var model = new EntityViewModel<Entity.UserAndRole.Admin>(lists, page, size);
            model.Items =
                await
                    model.Queryable.OrderBy(x => x.EmployeeCode)
                        .Skip((model.CurrentPage - 1) * size)
                        .Take(size)
                        .ToArrayAsync();

            ViewBag.NameOrCode = nameOrCode;
            ViewBag.Group = adminGroup;

            ViewBag.PartialViewPath = "_adminTable";

            return View("../../Views/MyAdmin/List", model);
        }

        //处理店铺和权限
        protected override async Task UpdateProperty(Entity.UserAndRole.Admin entity, IPropertyMetadata propertyMetadata)
        {
            if (propertyMetadata.ClrName == "Department")
            {
                var admin = (Session["AdminLogin"] as Entity.UserAndRole.Admin);
                var context = EntityBuilder.GetContext<Department>();
                propertyMetadata.SetValue(entity, context.GetEntity(admin.Department.Index));
            }
            else if (propertyMetadata.ClrName == "Group")
            {
                propertyMetadata.SetValue(entity, AdminGroup.业务员);
            }
            else
                await base.UpdateProperty(entity, propertyMetadata);
        }
    }
}