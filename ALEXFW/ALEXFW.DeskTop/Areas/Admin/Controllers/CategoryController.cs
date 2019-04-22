using ALEXFW.Entity.Gifts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class CategoryController : EntityController<Category>
    {
        public CategoryController(IEntityContextBuilder builder) : base(builder)
        {
        }

        public override async Task<ActionResult> Remove(Guid id)
        {
            var entity = EntityContext.GetEntity(id);

            //视图中需要显示的内容
            ViewBag.ObjectID = id;
            ViewBag.ObjectType = entity.Name;

            var context = EntityBuilder.GetContext<Product>();
            if (context.Query().Any(x => x.Category.Index == entity.Index))
            {
                Response.StatusCode = 400;
                return new ContentResult
                {
                    Content = entity.Name+"包含有礼品数据，不能删除"
                };
            }

            return await base.Remove(id);
        }
    }
}