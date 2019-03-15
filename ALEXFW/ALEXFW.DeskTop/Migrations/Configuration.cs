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
            //SeedMethod.Admin(context);
            //SeedMethod.Member(context);
            //SeedMethod.Mangager(context);
        }        
    }
}
