using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALEXFW.Entity.UserAndRole;
using ALEXFW.Entity;
using ALEXFW.Entity.Gifts;

namespace ALEXFW.DataAccess
{
    public class DBContext:DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet< Member> Members { get; set; }   
         
        //测试实体的上下文
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
