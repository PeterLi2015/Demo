using XDropsWater.Bll;
using XDropsWater.Bll.Interface;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XDropsWater.Web
{
    public class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

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
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }
    }

}