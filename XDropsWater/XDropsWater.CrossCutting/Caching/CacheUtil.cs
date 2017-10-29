using Microsoft.Practices.EnterpriseLibrary.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Caching
{
    public sealed class CacheUtil
    {
        private ICacheManager _CacheManager;
        private CacheUtil(string cacheContainerName)
        {
            this._CacheManager = CacheFactory.GetCacheManager(cacheContainerName);

            if (this._CacheManager == null)
                throw new ArgumentNullException("cacheContainerName is invalid.");
        }

        public int Count
        {
            get
            {
                return this._CacheManager.Count;
            }
        }

        public object this[string key]
        {
            get
            {
                return this[key];
            }
        }

        public void Add(string key, object value)
        {
            this._CacheManager.Add(key, value);
        }

        public void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
        {
            this._CacheManager.Add(key, value, scavengingPriority, refreshAction, expirations);
        }

        public bool Contains(string key)
        {
            return this._CacheManager.Contains(key);
        }

        public void Flush()
        {
            this._CacheManager.Flush();
        }

        public object GetData(string key)
        {
            return this._CacheManager.GetData(key);
        }

        public void Remove(string key)
        {
            this._CacheManager.Remove(key);
        }
    }
}
