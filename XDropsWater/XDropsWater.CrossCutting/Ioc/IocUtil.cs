using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Ioc
{
    /// <summary>
    /// The class initialize IOC Members
    /// </summary>
    public sealed class IocUtil
    {
        #region Fields

        private static readonly UnityContainer myContainer;
        private static readonly UnityConfigurationSection section = null;
        private const string DefaultContainerName = "application";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor 
        /// </summary>
        static IocUtil()
        {
            myContainer = new UnityContainer();
            section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(myContainer, DefaultContainerName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get concrete interface from configuration item
        /// </summary>
        /// <typeparam name="T">interface of Genenal type</typeparam>
        /// <param name="functionFactory">configuration item, get it from web.config file</param>
        /// <returns>interface of Genenal type</returns>
        public static T ConfigurateContainer<T>()
        {
            return myContainer.Resolve<T>();
        }

        #endregion
    }
}
