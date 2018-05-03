using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model.Register
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
