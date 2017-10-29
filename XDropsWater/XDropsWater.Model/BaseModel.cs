using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class BaseModel<T> : IBaseModel where T : struct
    {
        /// <summary>
        /// 唯一标识ID
        /// </summary>
        public T ID { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public Guid UpdateBy { get; set; }
        public DateTime UpdateOn { get; set; }
        public OpResult Result { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int No { get; set; }
    }
}
