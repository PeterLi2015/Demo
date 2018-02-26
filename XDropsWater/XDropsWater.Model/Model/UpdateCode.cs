using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 代理
    /// </summary>
    public class UpdateCode: BaseModel<int>
    {
        public long Code { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }
}
