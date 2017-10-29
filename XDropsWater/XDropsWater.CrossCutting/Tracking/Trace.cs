using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Tracking
{
    public class Trace
    {
        private readonly static bool _CanTrace = Convert.ToBoolean(ConfigurationManager.AppSettings["CanTrace"]);
        private Trace()
        {

        }

        public static void WriteLine(object value)
        {
            if (_CanTrace)
            {
                System.Diagnostics.Trace.WriteLine(value);
            }
        }

        public static void WriteLine(string message)
        {
            if (_CanTrace)
            {
                System.Diagnostics.Trace.WriteLine(message);
            }
        }

        public static void WriteLine(object value, string category)
        {
            if (_CanTrace)
            {
                System.Diagnostics.Trace.WriteLine(value, category);
            }
        }

        public static void WriteLine(string message, string category)
        {
            if (_CanTrace)
            {
                System.Diagnostics.Trace.WriteLine(message, category);
            }
        }
    }
}
