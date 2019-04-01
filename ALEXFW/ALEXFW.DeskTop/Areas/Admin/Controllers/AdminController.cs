using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.IO;
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
    }
}