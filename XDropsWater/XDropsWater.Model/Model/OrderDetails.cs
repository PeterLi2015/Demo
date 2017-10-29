using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// order details
    /// </summary>
    public class OrderDetails : BaseModel<Guid>
    {
        /// <summary>
        /// the order ID
        /// </summary>
        public Guid OrderID { get; set; }

        /// <summary>
        /// the product ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// the quantity of product
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 订单明细状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// the order object
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// the product object
        /// </summary>
        public virtual Product Product { get; set; }
    }
}
