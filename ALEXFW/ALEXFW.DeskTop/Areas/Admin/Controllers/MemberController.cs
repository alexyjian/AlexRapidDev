using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ALEXFW.CommonUtility;
using ALEXFW.Entity.Members;

namespace ALEXFW.DeskTop.Areas.Admin.Controllers
{
    public class MemberController : EntityController<Member>  
    {
        public MemberController(IEntityContextBuilder builder) : base(builder)
        {
        }

        //处理头像上传到本站
        protected override async Task UpdateProperty(Member entity, IPropertyMetadata propertyMetadata)
        {
            if (propertyMetadata.CustomType == "SingleImage")
            {
                var oldImage = this.UpSingleImage(entity);
                if (oldImage != entity.Avatar)
                    propertyMetadata.SetValue(entity, oldImage);
            }
            else
                await base.UpdateProperty(entity, propertyMetadata);
        }
    }
}