using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;
using System.Collections.Generic;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 父子关系表
    /// </summary>
    [Table("ParentChild")]
    public class ParentChildEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        public Guid ParentMemberID { get; set; }
        public Guid ChildMemberID { get; set; }
        public int ProvinceAgentCount { get; set; }
        public int GeneralAgentCount { get; set; }

        /// <summary>
        /// 市代理个数
        /// </summary>
        public int CityAgentCount { get; set; }
    }
}
