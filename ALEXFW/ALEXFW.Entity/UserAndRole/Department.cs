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
    [DisplayName("店铺")]
    [DisplayColumn("DepartmentName", "CreateDate", true)]
    [Parent(typeof(Department), "ParentDepartment")]
    public class Department : EntityBase
    {
        [Required]
        [Display(Name = "店铺编号")]
        public virtual string SortCode { get; set; }

        [Required]
        [Display(Name = "店铺名称")]
        public virtual string DepartmentName { get; set; } //部门名称

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