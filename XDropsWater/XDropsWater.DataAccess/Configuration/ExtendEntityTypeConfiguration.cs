
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using XDropsWater.Dal.DataAccess.Entity;
using XDropsWater.Dal.DataAccess.LamdaExpressions;
using XDropsWater.CrossCutting.Reflection;
namespace XDropsWater.DataAccess.Configuration
{
    public class ExtendEntityTypeConfiguration<TEntity, TID> :
        EntityTypeConfiguration<TEntity>, IEntityTypeConfiguration
        where TEntity : BaseEntity<TID>
        where TID : struct
    {
        public ExtendEntityTypeConfiguration()
        {
            this.EntityIDDatabaseGeneratedOption = DatabaseGeneratedOption.Identity;
        }

        /// <summary>
        /// don't call it,it is system fucntion
        /// </summary>
        public void InitBaseEntityMapConfig()
        {
            ///use inherit mode
            var map = this.Map(m =>
            {
                m.MapInheritedProperties();
                // the mapping. Note that the discriminator column is used to force EF to focus on undeleted entities. 
                //This adds the filtering predicate to all queries, including queries involving navigation properties.
                if (ReflectionUtil.HasInterface(typeof(TEntity), typeof(IDeleteLogEntity)))
                {
                    //filter deleted items
                    m.Requires("Deleted").HasValue(false);
                }
            });

            if (ReflectionUtil.HasInterface(typeof(TEntity), typeof(IConcurrencyEntity)))
            {
                var entityParam = ExpressionUtil.GetEntityTypeParam(typeof(TEntity));
                dynamic rowVersion = ExpressionUtil.GetEntityAttrLambdaExp(entityParam, "RowVersion", false);
                this.Property(rowVersion).IsConcurrencyToken();
            }
            this.HasKey(e => e.ID);
        }

        public DatabaseGeneratedOption EntityIDDatabaseGeneratedOption { get; set; }
    }
}
