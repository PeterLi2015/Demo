using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using Microsoft.Practices.Unity.InterceptionExtension;
using XDropsWater.Bll.Interface;
using XDropsWater.Bll;

namespace XDropsWater.Web
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            //Set the controller builder to use our custom controller factory
            var controllerFactory = new UnityControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    
            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.AddNewExtension<Interception>();
            container.RegisterType<IUserService, UserService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
                );
            container.RegisterType<IUserLogService, UserLogService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
                );
            container.RegisterType<IAchieveService, AchieveService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
                );
            container.RegisterType<IMemberService, MemberService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
                );
        }
    }
}