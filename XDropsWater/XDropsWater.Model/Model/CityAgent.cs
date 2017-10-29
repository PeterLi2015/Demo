using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 达到市级代理上级代理升级的时候减去多少套数量
    /// </summary>
    public class CityAgent
    {
        /// <summary>
        /// 市级代理数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 减去多少套
        /// </summary>
        public int Minus { get; set; }
    }
}
