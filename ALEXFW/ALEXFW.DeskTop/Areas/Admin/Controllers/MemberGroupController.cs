using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALEXFW.Entity.Members;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class MemberGroupController : EntityController<MemberGroup>
    {
        public MemberGroupController(IEntityContextBuilder builder) : base(builder)
        {
        }
    }
}
