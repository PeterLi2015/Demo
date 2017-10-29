using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class AchieveModel
    {
        public Guid ID { get; set; }

        public Guid MemberID { get; set; }

        public string MemberName { get; set; }

        public string MemberIdentityCardNo { get; set; }

        public decimal Amount { get; set; }

        public bool IsSubmit { get; set; }

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public Guid UpdateBy { get; set; }

        public DateTime UpdateOn { get; set; }

        public int BatchNumber { get; set; }

        public string ParentName { get; set; }

        public string ParentIdentityCardNo { get; set; }

        /// <summary>
        /// 1新代理入单2老代理补单3老代理消费
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 积分，2800为1分
        /// </summary>
        public int Score { get; set; }

        
    }
}
