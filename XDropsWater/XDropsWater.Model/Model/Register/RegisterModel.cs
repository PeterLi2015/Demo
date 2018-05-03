using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Register = XDropsWater.Model.Register;

namespace XDropsWater.Model
{
    public class RegisterModel
    {
        public Register.User User { get; set; }
        public Register.Member Member { get; set; }
        public Register.MemberAddress MemberAddress { get; set; }
        public Register.Product Product { get; set; }

    }
}
