using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.Entity.Gifts
{
    [DisplayName("商品")]
    [DisplayColumn("Name", "SortCode", false)]
    //权限
    [EntityAuthentication(AllowAnonymous = false,
        RemoveRolesRequired = new object[] { AdminGroup.管理员})]
    //树型筛选
    [Parent(typeof(Category),"Category")]
    public class Product : EntityBase
    {
        [Required(ErrorMessage = "商品名称不能为空")]
        [Display(Name = "商品名称", Order = 2)]
        public virtual string Name { get; set; }

        [Required(ErrorMessage = "商品编号不能为空")]
        [Display(Name = "商品编号", Order = 1)]
        public virtual string SortCode { get; set; }

        [Display(Name = "商品说明", Order = 10)]
        [Hide(IsHiddenOnCreate = false, IsHiddenOnEdit = false)]
        public virtual string Description { get; set; }


        [Display(Name = "商品价格", Order = 4)]
        [Required(ErrorMessage = "商品价格不能为空")]
        public virtual decimal Price { get; set; } = 0.00M;

        //与分类关联
        [Display(Name = "商品分类", Order = 4)]
        [Required(ErrorMessage = "分类不能为空")]
        public virtual Category Category { get; set; }

        public override void OnCreateCompleted()
        {
            base.OnCreateCompleted();
            CreateDate = DateTime.Now;
        }
    }
}
