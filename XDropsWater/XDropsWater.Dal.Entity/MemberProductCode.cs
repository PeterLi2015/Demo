using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 会员产品识别码
    /// </summary>
    [Table("MemberProductCode")]
    public class MemberProductCodeEntity: BaseEncryptCreateUpdateLogEntity<Guid>
    {
        public Guid MemberProductID { get; set; }
        public long Code { get; set; }
        public virtual MemberProductEntity MemberProduct { get; set; }
    }
}
