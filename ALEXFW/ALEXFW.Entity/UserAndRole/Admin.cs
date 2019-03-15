using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace ALEXFW.Entity.UserAndRole
{
    //[EntityAuthentication(AllowAnonymous = false,
    //    AddRolesRequired = new object[] { AdminGroup.超级管理员 },
    //    EditRolesRequired = new object[] { AdminGroup.超级管理员 },
    //    RemoveRolesRequired = new object[] { AdminGroup.超级管理员 })]
    [DisplayName("管理员")]
    [DisplayColumn("Username", "CreateDate", true)]
    [Parent(typeof(Department), "Department")]
    public class Admin : UserBase
    {
        [Display(Name = "工号", Order = 0)]
        [Searchable]
        public virtual string EmployeeCode { get; set; } // 工号

        [Required]
        [Display(Name = "用户名", Order = 1)]
        [Searchable]
        public virtual string Username { get; set; }

        [Display(Name = "密码", Order = 2)]
        public override byte[] Password
        {
            get { return base.Password; }
            set { base.Password = value; }
        }

        [Searchable]
        [Display(Name = "分组", Order = 4)]
        public virtual Department Department { get; set; }

        [Display(Name = "是否锁定", Order = 20)]
        public virtual bool IsLocked { get; set; } = false;//是否锁定，不能登录

        [Display(Name = "是否删除", Order = 30)]
        public virtual bool IsDeleted { get; set; } = false;//是否锁定，不能登录

        [Hide(IsHiddenOnCreate = true, IsHiddenOnEdit = true)]
        public virtual DateTime LastLoginDateTime { get; set; } = DateTime.Now; //上一次登录时间

        [Required]
        [Display(Name = "用户权限", Order = 20)]
        [Searchable]
        public virtual AdminGroup Group { get; set; }

        public override bool IsInRole(object role)
        {
            if (role is string)
            {
                AdminGroup value;
                if (!Enum.TryParse((string)role, out value))
                    return false;
                return Group.HasFlag(value);
            }
            if (role is AdminGroup)
                return Group.HasFlag((AdminGroup)role);
            return false;
        }
    }
}