using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    [Table("Logs")]
    public class LogEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        public string ErrMsg { get; set; }
        [MaxLength(200)]
        public string ModuleName { get; set; }
    }
}
