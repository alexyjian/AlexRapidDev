using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ALEXFW.Entity.UserAndRole
{
    [EntityAuthentication(AllowAnonymous = false,
        AddRolesRequired = new object[] { AdminGroup.管理员 },
        RemoveRolesRequired = new object[] { "NotAllowed" },
        EditRolesRequired = new object[] { AdminGroup.管理员 })]
    [DisplayName("会员")]
    [DisplayColumn("PersonName", "CreateDate", true)]
    [Parent(typeof(Department), "Department")]
    public class Member : UserBase
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [Searchable]
        [Display(Name = "用户名", Order = 0)]
        public virtual string Username { get; set; }

        [Required]
        [Searchable]
        [Display(Name = "姓名", Order = 1)]
        public virtual string PersonName { get; set; }
        
        [Display(Name = "密码", Order = 2)]
        public override byte[] Password
        {
            get { return base.Password; }
            set { base.Password = value; }
        }
        
        [Searchable]
        [Display(Name = "分组", Order = 4)]
        public virtual Department Department { get; set; }

        [Display(Name = "头像", Order = 105)]
        [CustomDataType("SingleImage")]
        [Hide(IsHiddenOnCreate = false, IsHiddenOnEdit = false)]
        public virtual string Avatar { get; set; }

        [Display(Name = "是否禁用", Order = 5)]
        [CustomDataType(CustomDataType.Boolean)]
        public virtual bool IsEnabled { get; set; }

        [Display(Name = "上次登录时间", Order = 111)]
        [Column(TypeName = "datetime2")]
        [Hide]
        public virtual DateTime LastLoginDateTime { get; set; }

        public string GetName()
        {
            if (string.IsNullOrEmpty(PersonName))
                return PersonName ?? Username;
            return PersonName;
        }
    }
}