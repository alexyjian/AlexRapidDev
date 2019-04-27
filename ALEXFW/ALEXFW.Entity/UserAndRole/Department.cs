using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace ALEXFW.Entity.UserAndRole
{
    /// <summary>
    ///   店铺
    /// </summary>
    [EntityAuthentication(AllowAnonymous = false,
        AddRolesRequired = new object[] { AdminGroup.管理员 },
        EditRolesRequired = new object[] { AdminGroup.管理员 },
        RemoveRolesRequired = new object[] { AdminGroup.管理员 },
        ViewRolesRequired = new object[]{AdminGroup.管理员})]
    [DisplayName("店铺")]
    [DisplayColumn("DepartmentName", "CreateDate", true)]
    public class Department : EntityBase
    {
        [Required]
        [Display(Name = "店铺编号", Order = 1)]
        [Searchable]
        public virtual string SortCode { get; set; }

        [Required]
        [Display(Name = "店铺名称", Order = 2)]
        [Searchable]
        public virtual string DepartmentName { get; set; } //部门名称

        [Display(Name = "店铺简介", Order = 3)]
        [Hide]
        public virtual string DSCN { get; set; }

        [Display(Name = "Logo", Order =4)]
        [Hide]
        public virtual string Logo { get; set; }

        [Hide]
        [Display(Name = "店铺人员")]
        public virtual ICollection<Admin> Admins { get; set; } //店铺人员

        public override void OnCreateCompleted()
        {
            base.OnCreateCompleted();
            CreateDate = DateTime.Now;
        }
    }
}