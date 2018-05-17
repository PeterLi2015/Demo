using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity.Lifetime;

namespace XDropsWater.Web
{
    public class PerRequestLifetimeManager : LifetimeManager
    {
        private readonly string sessionKey;
        public PerRequestLifetimeManager(string sessionKey)
        {
            this.sessionKey = sessionKey;
        }
        public override object GetValue(ILifetimeContainer container = null)
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Items == null || HttpContext.Current.Items.Count == 0)
            {
                return null;
            }
            var key = sessionKey + "-" + HttpContext.Current.Session.SessionID;
            return HttpContext.Current.Items[key];
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            if (HttpContext.Current.Session != null)
            {
                var key = sessionKey + "-" + HttpContext.Current.Session.SessionID;
                HttpContext.Current.Items[key] = newValue;
            }
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
            if (HttpContext.Current.Items != null)
            {
                var key = sessionKey + "-" + HttpContext.Current.Session.SessionID;
                HttpContext.Current.Items[key] = null;
            }
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            throw new NotImplementedException();
        }
    }
}