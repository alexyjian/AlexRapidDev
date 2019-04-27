using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.Entity.Members
{
    [EntityAuthentication(AllowAnonymous = false,
        AddRolesRequired = new object[] {AdminGroup.管理员},
        EditRolesRequired = new object[] {AdminGroup.管理员},
        RemoveRolesRequired = new object[] {AdminGroup.管理员})]
    [DisplayName("会员组")]
    [DisplayColumn("Name", "Order")]
    public class MemberGroup : EntityBase
    {
        [Searchable]
        [Display(Name = "排序", Order = 0)]
        public virtual int SortCode { get; set; }

        [Searchable]
        [Display(Name = "名称", Order = 10)]
        public virtual string Name { get; set; }


        [Hide(IsHiddenOnView = false, IsHiddenOnDetail = false)]
        [Display(Name = "人数", Order = 20)]
        public int Count
        {
            get { return Members.Count; }
        }

        [Hide]
        public virtual ICollection<Member> Members { get; set; }
    }
}
