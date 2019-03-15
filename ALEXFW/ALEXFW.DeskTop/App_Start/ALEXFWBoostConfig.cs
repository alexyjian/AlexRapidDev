using Microsoft.Practices.Unity;
using System;
using System.Data.Entity;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ALEXFW.DeskTop.App_Start.ALEXFWBoostConfig), "Start")]

namespace ALEXFW.DeskTop.App_Start
{
    public class ALEXFWBoostConfig
    {
        public static void Start()
        {
            System.Web.Security.ALEXFWPrincipal.Resolver = Resolver;
        }

        private static IRoleEntity Resolver(Type entityType, string username)
        {
            Guid id;
            if (!Guid.TryParse(username, out id))
                return null;
            var builder = UnityConfig.GetConfiguredContainer().Resolve<IEntityContextBuilder>();
            dynamic context = builder.GetContext(entityType);
            var member = context.GetEntity(id);
            return member;
        }
    }
}