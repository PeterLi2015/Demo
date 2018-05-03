using System.Web.Mvc;
using XDropsWater.Bll.Interface;
using XDropsWater.Bll;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess;
using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Lifetime;
using Unity.Injection;

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
            container.RegisterType<IRegisterService, RegisterService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
                );
            container.RegisterType<ICacheService, CacheService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
                );
            container.RegisterType<IMemberAddressService, MemberAddressService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>()
                );
            container.RegisterType<IUnitOfWork, SimpleWebUnitOfWork>(
                new SessionLifetimeManager("uow")
                );
            
            container.RegisterType<IProvince, Province>(
                new ContainerControlledLifetimeManager()
                );
            
            container.RegisterType<IRepository<AddressEntity>, Repository<AddressEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IUnitOfWork, SimpleWebUnitOfWork>(
                "transient"
                );
            container.RegisterType<IRepository<MemberAddressEntity>, Repository<MemberAddressEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<MemberProductEntity>, Repository<MemberProductEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<ProductEntity>, Repository<ProductEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<MemberEntity>, Repository<MemberEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<UserEntity>, Repository<UserEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<OrderEntity>, Repository<OrderEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<SystemConfigEntity>, Repository<SystemConfigEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<MemberRoleEntity>, Repository<MemberRoleEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<RoleUpgradeEntity>, Repository<RoleUpgradeEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<OrderDetailsEntity>, Repository<OrderDetailsEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );
            container.RegisterType<IRepository<ShoppingCartEntity>, Repository<ShoppingCartEntity>>(
                new InjectionConstructor(typeof(IUnitOfWork))
                );

            container.RegisterType<IRegisterObserver, UserService>("UserService");
            container.RegisterType<IRegisterObserver, OrderService>("OrderService");
            container.RegisterType<IRegisterObserver, MemberService>("MemberService");
            container.RegisterType<IRegisterObserver, MemberAddressService>("MemberAddressService");
            container.RegisterType<IRegisterObserver, MemberProductService>("MemberProductService");
            container.CacheAddresses();
        }
    }
}