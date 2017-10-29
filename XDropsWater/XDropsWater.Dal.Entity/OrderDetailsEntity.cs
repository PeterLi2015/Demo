using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// show order details
    /// </summary>
    [Table("OrderDetails")]
    public class OrderDetailsEntity : BaseEncryptCreateUpdateLogEntity<Guid>
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
        /// 0没有识别码，1有识别码未填满，2有识别码已填满
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// the order object
        /// </summary>
        public virtual OrderEntity Order { get; set; }

        /// <summary>
        /// the product object
        /// </summary>
        public virtual ProductEntity Product { get; set; }
    }
}
