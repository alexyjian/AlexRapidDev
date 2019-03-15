using ALEXFW.Entity.Demos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class CategoryController :EntityController<Category>
    {
        public CategoryController(IEntityContextBuilder builder):base(builder)
        {
        }
    }
}