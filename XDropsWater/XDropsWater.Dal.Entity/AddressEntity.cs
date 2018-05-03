using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;
using System.Collections.Generic;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理表
    /// </summary>
    [Table("Address")]
    public class AddressEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 省份
        /// </summary>
         [MaxLength(20)]
        public string Province { get; set; }

        [MaxLength(50)]
        public string ProvinceCode { get; set; }

        public string JsonValue { get; set; }
    }
}
