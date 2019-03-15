using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace ALEXFW.Entity.UserAndRole
{
    /// <summary>
    ///     部门
    /// </summary>
    [DisplayName("员工部门")]
    [DisplayColumn("DepartmentName", "CreateDate", true)]
    [Parent(typeof(Department), "ParentDepartment")]
    public class Department : EntityBase
    {
        [Required]
        [Display(Name = "部门编号")]
        public virtual string SortCode { get; set; }

        [Required]
        [Display(Name = "部门名称")]
        public virtual string DepartmentName { get; set; } //部门名称

        [Hide]
        [Display(Name = "员工")]
        public virtual ICollection<Admin> Admins { get; set; } //部门员工

        [Display(Name = "上级部门")]
        public virtual Department ParentDepartment { get; set; } //上级部门

        public override void OnCreateCompleted()
        {
            base.OnCreateCompleted();
            CreateDate = DateTime.Now;
        }
    }
}