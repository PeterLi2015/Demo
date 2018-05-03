using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class AddressModel : BaseModel<Guid>
    {
        public string Province { get; set; }

        public string ProvinceCode { get; set; }

        public string JsonValue { get; set; }
    }
}
