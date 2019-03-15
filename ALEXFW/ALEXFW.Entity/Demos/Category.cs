using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.Entity.Demos
{
    /// <summary>
    /// 商品分类的实体
    /// </summary>
    //实体名称用中文
    [DisplayName("商品分类")]   
    //实例缺省显示字段，缺省排序字段，缺省排序方式
    [DisplayColumn("Name","SortCode",false)]
    //实体的权限控制：
    //AllowAnonymous  匿名用户访问本实体权限
    //AddRolesRequired 只有管理员权限可添加
    //RemoveRolesRequired 只有管理员权限可以删除
    //EditRolesRequired 只有管理员可以编辑
    [EntityAuthentication(AllowAnonymous = false,    
        AddRolesRequired = new object[]{ AdminGroup.管理员 }, 
        RemoveRolesRequired = new object[] { AdminGroup.管理员 },  
        EditRolesRequired = new object[] { AdminGroup.管理员 })]  
    public class Category:EntityBase
    {
        //框架要求属性全部需要有virtual

        //分类的名称，字段排序权值 
        [Display(Name = "分类名称", Order = 1)]
        [Required(ErrorMessage = "分类名称不能为空")]
        public virtual string Name { get; set; }

        [Display(Name = "说明", Order = 10)]
        //字段的隐藏 按需求在不同的视图显示 
        [Hide(IsHiddenOnCreate = false,IsHiddenOnDetail = false,IsHiddenOnEdit = false)]
        public virtual string Description { get; set; }

        //分类的编号
        [Required(ErrorMessage = "分类编号不能为空")]
        [Display(Name = "分类编号",Order = 0)]
        public virtual  string SortCode { get; set; }

        //希望能自动记录每一条数据添加的时间
        public override void OnCreateCompleted()
        {
            base.OnCreateCompleted();
            //在此对象被创建时，保存当前时间
            CreateDate = DateTime.Now;   
        }
    }
}
