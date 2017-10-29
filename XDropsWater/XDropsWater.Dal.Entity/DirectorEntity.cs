using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;
using System.Collections.Generic;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 董事表
    /// </summary>
    [Table("Director")]
    public class DirectorEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        public Guid MemberID { get; set; }

        /// <summary>
        /// 代理
        /// </summary>
        [ForeignKey("MemberID")]
        public MemberEntity Member { get; set; }

        /// <summary>
        /// 董事序号
        /// </summary>
        public int DirectorNo { get; set; }
    }
}
