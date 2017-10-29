
using System.ComponentModel.DataAnnotations.Schema;
namespace XDropsWater.DataAccess.Configuration
{
    public interface IEntityTypeConfiguration
    {
        void InitBaseEntityMapConfig();
        DatabaseGeneratedOption EntityIDDatabaseGeneratedOption { get; set; }
    }
}
