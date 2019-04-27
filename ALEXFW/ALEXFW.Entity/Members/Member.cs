using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.Entity.Members
{
    [EntityAuthentication(AllowAnonymous = false,
        AddRolesRequired = new object[] { AdminGroup.业务员 },
        RemoveRolesRequired = new object[] { "NotAllowed" },
        EditRolesRequired = new object[] { AdminGroup.业务员 })]
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

        [Display(Name = "性别", Order = 2)]
        [CustomDataType(CustomDataType.Sex)]
        [Hide(IsHiddenOnCreate = false, IsHiddenOnEdit = false)]

        public virtual bool? Sex { get; set; }

        [Display(Name = "生日", Order = 3)]
        [Hide(IsHiddenOnCreate = false, IsHiddenOnEdit = false)]
        [Column(TypeName = "datetime2")]
        public virtual DateTime? Birthday { get; set; }

        [Display(Name = "手机", Order = 4)]
        [Hide(IsHiddenOnCreate = false, IsHiddenOnEdit = false)]
        [CustomDataType(CustomDataType.PhoneNumber)]
        [Searchable]
        public virtual string Phone { get; set; }

        [Display(Name = "密码", Order = 10)]
        [Hide(IsHiddenOnCreate = false, IsHiddenOnEdit = false)]
        public override byte[] Password
        {
            get { return base.Password; }
            set { base.Password = value; }
        }
       
        [Display(Name = "店铺", Order = 20)]
        [PropertyAuthentication(EditRolesRequired = new object[] { AdminGroup.管理员 },
            ViewRolesRequired = new object[] { AdminGroup.店长 })]
        public virtual Department Department { get; set; }

        [Display(Name = "是否禁用", Order = 30)]
        [CustomDataType(CustomDataType.Boolean)]
        public virtual bool IsEnabled { get; set; } = true;

        [Display(Name = "分组", Order = 5)]
        public virtual MemberGroup Group { get; set; }

        [Display(Name = "头像", Order = 105)]
        [CustomDataType("SingleImage")]
        [Hide(IsHiddenOnCreate = false, IsHiddenOnEdit = false)]
        public virtual string Avatar { get; set; }


        [Display(Name = "上次登录时间", Order = 100)]
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