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
    /// 产品表
    /// </summary>
    [Table("Products")]
    public class ProductEntity : BaseEncryptCreateUpdateLogEntity<int>
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// the img path
        /// </summary>
        public string ImgSrc { get; set; }

        /// <summary>
        /// the img description
        /// </summary>
        public string ImgAlt { get; set; }

        /// <summary>
        /// 是否有唯一识别码
        /// </summary>
        public bool HasIdentityCode { get; set; }
    }
}
