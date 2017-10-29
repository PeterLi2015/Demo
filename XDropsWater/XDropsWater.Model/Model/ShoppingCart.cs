using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class ShoppingCart : BaseModel<Guid>
    {
        public Guid MemberID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public Member Member { get; set; }
        public Product Product { get; set; }
    }
}
