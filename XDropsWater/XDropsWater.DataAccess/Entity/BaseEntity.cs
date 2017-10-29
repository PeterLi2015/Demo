
using XDropsWater.DataAccess.Configuration;
namespace XDropsWater.Dal.DataAccess.Entity
{
    /// <summary>
    /// base attributions for all entity
    /// </summary>
    public class BaseEntity<TID> where TID : struct
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseEntity()
        {

        }

        /// <summary>
        /// 唯一标识ID
        /// </summary>
        public TID ID { get; set; }


        #region mapping
        protected virtual void ConfigMap(IEntityTypeConfiguration config)
        {
            config.InitBaseEntityMapConfig();
        }
        #endregion
    }
}
