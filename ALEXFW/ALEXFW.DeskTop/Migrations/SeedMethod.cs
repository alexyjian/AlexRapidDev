using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ALEXFW.CommonUtility;
using ALEXFW.DataAccess;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop.Migrations
{
    public class SeedMethod
    {
        public static void Admin(DBContext context)
        {
            Admin admin = new Admin();
            admin.Index = Guid.NewGuid();
            admin.CreateDate = DateTime.Now;
            admin.Username = "admin";
            admin.SetPassword("admin");
            admin.Group = AdminGroup.管理员 | AdminGroup.总经理 | AdminGroup.经理 | AdminGroup.主管 | AdminGroup.员工;
            admin.EmployeeCode = "0001";
            admin.IsDeleted = false;
            admin.IsLocked = false;
            context.Admins.Add(admin);
            context.SaveChanges();
        }

        /// <summary>
        /// 添加一名管理员用户，权限是总经理
        /// </summary>
        /// <param name="context"></param>
        public static void Mangager(DBContext context)
        {
            Admin admin = new Admin();
            admin.Index = Guid.NewGuid();
            admin.CreateDate = DateTime.Now;
            admin.Username = "ceo";
            admin.SetPassword("123.abc");
            admin.Group = AdminGroup.总经理 | AdminGroup.经理 | AdminGroup.主管 | AdminGroup.员工;
            admin.EmployeeCode = "1001";
            admin.IsDeleted = false;
            admin.IsLocked = false;
            context.Admins.Add(admin);
            context.SaveChanges();
        }

        public static void Member(DBContext context)
        {
            Member member = new Member();
            member.Index = Guid.NewGuid();
            member.Username = "Alex";
            member.CreateDate = DateTime.Now;
            member.LastLoginDateTime = DateTime.Now;
            member.PersonName = "余剑";
            member.SetPassword("123456");
            member.Avatar = "";
            context.Members.Add(member);
            context.SaveChanges();

            Member member1 = new Member();
            member1.Index = Guid.NewGuid();
            member1.Username = "messi";
            member1.CreateDate = DateTime.Now;
            member1.LastLoginDateTime = DateTime.Now;
            member1.PersonName = "梅西";
            member1.SetPassword("123456");
            member1.Avatar = "";
            context.Members.Add(member1);
            context.SaveChanges();
        }
    }
}