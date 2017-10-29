using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 奖金表
    /// </summary>
    [Table("Bonus")]
    public class BonusEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
       
        /// <summary>
        /// 代理ID
        /// </summary>
        [Required]
        public Guid? MemberID { get; set; }

        /// <summary>
        /// 年月
        /// </summary>
        [MaxLength(6)]
        public string YearMonth { get; set; }

        /// <summary>
        /// 全国互动分红奖
        /// </summary>
        public decimal? InteractiveBonus { get; set; }

        /// <summary>
        /// 直推奖
        /// </summary>
        public decimal? DirectRecommendBonus { get; set; }

        /// <summary>
        /// 网络服务奖
        /// </summary>
        public decimal? NetworkServiceBonus { get; set; }

        /// <summary>
        /// 辅助奖
        /// </summary>
        public decimal? AssistanceBonus { get; set; }

        /// <summary>
        /// 杰出贡献奖
        /// </summary>
        public decimal? OutstandingContributionBonus { get; set; }

        /// <summary>
        /// 区代理奖
        /// </summary>
        public decimal? DistrictAgencyBonus { get; set; }

        /// <summary>
        /// 市代理奖
        /// </summary>
        public decimal? CityAgencyBonus { get; set; }

        /// <summary>
        /// 省代理奖
        /// </summary>
        public decimal? ProvinceAgencyBonus { get; set; }

        /// <summary>
        /// 省代理分红奖
        /// </summary>
        public decimal? ProvinceAgencySharingBonus { get; set; }

        /// <summary>
        /// 工作室补助将
        /// </summary>
        public decimal? StoreGrantBonus { get; set; }

        /// <summary>
        /// 超销奖
        /// </summary>
        public decimal? OverSaleBonus { get; set; }
    }
}
