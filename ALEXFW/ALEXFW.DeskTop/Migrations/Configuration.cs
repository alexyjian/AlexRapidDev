namespace ALEXFW.DeskTop.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ALEXFW.DataAccess.DBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ALEXFW.DataAccess.DBContext context)
        {
            //context.Database.ExecuteSqlCommand("delete admins");
            //context.Database.ExecuteSqlCommand("delete members");
            //context.Database.ExecuteSqlCommand("delete departments");
            //SeedMethod.Department(context);
            //SeedMethod.Admin(context);
            //SeedMethod.Manage(context);
            //SeedMethod.Clerk(context);
            //SeedMethod.Member(context);
        }        
    }
}
