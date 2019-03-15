using ALEXFW.Entity.UserAndRole;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class DepartmentController : EntityController<Department>
    {
        public DepartmentController(IEntityContextBuilder builder) : base(builder)
        {
        }
    }
}