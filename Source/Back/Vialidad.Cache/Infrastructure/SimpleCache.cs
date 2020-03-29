using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;

namespace Vialidad.Cache.Infrastructure
{
    static class SimpleCache
    {
        #region Private Attributes
        private static ObjectCache _cache;
        private static ILogger _logger;
        #endregion

        #region Constructors
        static SimpleCache()
        {
            _cache = MemoryCache.Default;
            _logger = LoggerFactory.GetInstance();
        }
        #endregion

        #region Public Methods
        public static TObject GetCache<TObject>(string key, Func<TObject> getObject)
        {
            TObject cachedObject = default(TObject);
            try
            {
                if (_cache.Contains(key))
                    cachedObject = (TObject)_cache[key];

                if (cachedObject == null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(CacheCommonSettings.CacheMinutes);
                    cachedObject = getObject();
                    _cache.Set(key, cachedObject, policy);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SimpleCache.GetCache", ex.Message, ex);
            }
            return cachedObject;
        }

        public static void CleanCache()
        {
            try
            {
                foreach (var itemCache in _cache)
                    _cache.Remove(itemCache.Key);
            }
            catch (Exception ex)
            {
                _logger.Error("SimpleCache.CleanCache", ex.Message, ex);
            }
        }

        public static void CleanCacheService(string keyPrefix)
        {
            try
            {
                foreach (var itemCache in _cache)
                    if (itemCache.Key.ToLowerInvariant().StartsWith(keyPrefix.ToLowerInvariant()))
                        _cache.Remove(itemCache.Key);
            }
            catch (Exception ex)
            {
                _logger.Error("SimpleCache.CleanCacheService", ex.Message, ex);
            }
        }

        public static void CleanCacheItem(string key)
        {
            try
            {
                if (_cache.Contains(key))
                    _cache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.Error("SimpleCache.CleanCacheItem", ex.Message, ex);
            }
        }
        #endregion
    }
}
