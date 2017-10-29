using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// product object
    /// </summary>
    public class Product : BaseModel<int>
    {
        /// <summary>
        /// product name
        /// </summary>
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
