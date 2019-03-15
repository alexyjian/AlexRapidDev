using Microsoft.Practices.Unity;
using System;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ALEXFW.DataAccess;
using ALEXFW.DeskTop.App_Start;
using ALEXFW.Entity.UserAndRole;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityControllerFactory), "Start")]

namespace ALEXFW.DeskTop.App_Start
{
    public class UnityControllerFactory : EntityControllerFactory
    {
        private readonly IUnityContainer _container;

        public UnityControllerFactory(IUnityContainer container)
        {
            _container = container;

            //Change EntityContextBuilder to your class that inherit IEntityContextBuilder interface.
            //If your entity context builder has constructor with arguments, continue register types that you need.
            container.RegisterType<DbContext, DBContext>(new PerRequestLifetimeManager());
            container.RegisterType<IEntityContextBuilder, EntityContextBuilder>(new PerRequestLifetimeManager());

            //Register your entity here:
            //RegisterController<EntityType>();

            System.Web.Security.ALEXFWPrincipal.Resolver = EntityResolve;
        }

        private IRoleEntity EntityResolve(Type entityType, string username)
        {
            IEntityContextBuilder builder = _container.Resolve<IEntityContextBuilder>();
            dynamic context = builder.GetContext(entityType);
            return context.GetEntity(new Guid(username));
        }

        public static void Start()
        {
            ControllerBuilder.Current.SetControllerFactory(
                new UnityControllerFactory(UnityConfig.GetConfiguredContainer()));
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, "Controller Not Found.");
            }
            return _container.Resolve(controllerType) as IController;
        }
    }
}