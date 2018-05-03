using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Unity;

namespace XDropsWater.Web
{
    public class UnityControllerFactory : DefaultControllerFactory
    {
        IUnityContainer container;
        public UnityControllerFactory(IUnityContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext reqContext,
            Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }
            return container.Resolve(controllerType) as IController;

        }
    }
}