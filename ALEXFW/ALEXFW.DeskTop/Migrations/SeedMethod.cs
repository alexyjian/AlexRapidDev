using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ALEXFW.CommonUtility;
using ALEXFW.DataAccess;
using ALEXFW.Entity.Members;
using ALEXFW.Entity.UserAndRole;

namespace ALEXFW.DeskTop.Migrations
{
    public class SeedMethod
    {
        /// <summary>
        /// 店铺
        /// </summary>
        /// <param name="context"></param>
        public static void Department(DBContext context)
        {
            var d1 = new Department()
                {Index = Guid.NewGuid(), SortCode = "0001", DepartmentName = "柳北店", CreateDate = DateTime.Now};
            var d2 = new Department()
                {Index = Guid.NewGuid(), SortCode = "0002", DepartmentName = "柳南店", CreateDate = DateTime.Now};
            var d3 = new Department()
                {Index = Guid.NewGuid(), SortCode = "0003", DepartmentName = "柳东店", CreateDate = DateTime.Now};
            var d4 = new Department()
                {Index = Guid.NewGuid(), SortCode = "0004", DepartmentName = "河西店", CreateDate = DateTime.Now};
            context.Departments.Add(d1);
            context.Departments.Add(d2);
            context.Departments.Add(d3);
            context.Departments.Add(d4);
            context.SaveChanges();
        }
        public static void Admin(DBContext context)
        {
            Admin admin = new Admin();
            admin.CreateDate = DateTime.Now;
            admin.Index = Guid.NewGuid();
            admin.CreateDate = DateTime.Now;
            admin.Username = "admin";
            admin.SetPassword("admin");
            admin.Group = AdminGroup.管理员 | AdminGroup.店长 | AdminGroup.业务员;
            admin.EmployeeCode = "0001";
            admin.IsLocked = false;
            context.Admins.Add(admin);
            context.SaveChanges();
        }

        /// <summary>
        /// 添加一名店长用户，权限是店长
        /// </summary>
        /// <param name="context"></param>
        public static void Manage(DBContext context)
        {
            Admin admin = new Admin();
            admin.CreateDate = DateTime.Now;
            admin.Index = Guid.NewGuid();
            admin.CreateDate = DateTime.Now;
            admin.Username = "Mike";
            admin.SetPassword("123.abc");
            admin.Group =  AdminGroup.店长 | AdminGroup.业务员;
            admin.EmployeeCode = "1001";
            admin.IsLocked = false;
            admin.Department = context.Departments.SingleOrDefault(x => x.DepartmentName == "柳北店");
            context.Admins.Add(admin);
            context.SaveChanges();
        }

        /// <summary>
        /// 添加一名业务员用户，权限是业务员
        /// </summary>
        /// <param name="context"></param>
        public static void Clerk(DBContext context)
        {
            Admin admin = new Admin();
            admin.CreateDate = DateTime.Now;
            admin.Index = Guid.NewGuid();
            admin.CreateDate = DateTime.Now;
            admin.Username = "Tom";
            admin.SetPassword("123.abc");
            admin.Group = AdminGroup.业务员;
            admin.EmployeeCode = "2001";
            admin.IsLocked = false;
            admin.Department = context.Departments.SingleOrDefault(x => x.DepartmentName == "柳北店");
            context.Admins.Add(admin);
            context.SaveChanges();
        }


        /// <summary>
        /// 会员用户
        /// </summary>
        /// <param name="context"></param>
        public static void Member(DBContext context)
        {
            Member member = new Member();
            member.Index = Guid.NewGuid();
            member.Username = "Alex";
            member.CreateDate = DateTime.Now;
            member.LastLoginDateTime = DateTime.Now;
            member.PersonName = "余剑";
            member.SetPassword("123456");
            member.Avatar = "/images/avadar/avadar.jpg";
            member.Department = context.Departments.SingleOrDefault(x => x.DepartmentName == "柳北店");
            member.Phone = "13807723369";
            context.Members.Add(member);
            context.SaveChanges();

            Member member1 = new Member();
            member1.Index = Guid.NewGuid();
            member1.Username = "Messi";
            member1.CreateDate = DateTime.Now;
            member1.LastLoginDateTime = DateTime.Now;
            member1.PersonName = "梅西";
            member1.SetPassword("123456");
            member1.Department = context.Departments.SingleOrDefault(x => x.DepartmentName == "柳北店");
            member1.Avatar = "/images/avadar/avadar.jpg";
            member1.Phone = "13907723339";
            context.Members.Add(member1);
            context.SaveChanges();
        }
    }
}