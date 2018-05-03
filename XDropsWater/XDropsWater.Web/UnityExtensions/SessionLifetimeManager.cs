using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using Unity.Interception.Utilities;
using Unity.Lifetime;

namespace XDropsWater.Web
{
    public class SessionLifetimeManager : LifetimeManager
    {
        private readonly string sessionKey;
        public SessionLifetimeManager(string sessionKey)
        {
            this.sessionKey = sessionKey;
        }
        public override object GetValue(ILifetimeContainer container = null)
        {
            if (HttpContext.Current.Session == null)
            {
                return null;
            }
            var key = sessionKey + "-" + HttpContext.Current.Session.SessionID;
            return HttpContext.Current.Session[key];
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            if (HttpContext.Current.Session != null)
            {
                var key = sessionKey + "-" + HttpContext.Current.Session.SessionID;
                HttpContext.Current.Session[key] = newValue;
            }
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
            if (HttpContext.Current.Session != null)
            {
                var key = sessionKey + "-" + HttpContext.Current.Session.SessionID;
                HttpContext.Current.Session[key] = null;
            }
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            throw new NotImplementedException();
        }
    }
    
}