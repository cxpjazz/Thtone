using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CoreHelper
{
    /// <summary>
    /// 数据缓存，用于Web程序
    /// </summary>
    public class CacheHelper
    {
        private static readonly Object _locker = new object();
        #region 获取数据缓存
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="CacheKey">键</param>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey">缓存名称</param>
        /// <param name="cachePopulate">
        /// lamda表达式:可以是() => new BLL.CangFangList().Single(CangFangId)或者() => "abc"
        /// 或者委托(注意返回)： delegate() { return new BLL.CangFangList().Single(CangFangId); }
        /// </param>
        /// <param name="slidingExpiration">用于设置可调过期时间，它表示当离最后访问超过某个时间段后就过期new TimeSpan(0, 10, 0)或者TimeSpan.FromMinutes(60)</param>
        /// <remarks>
        /// var value = CoreHelper.CacheHelper.GetCache<List<Model.S_MenuList>>
        ///             (
        ///              string.Format("menu{0}", currentUser.Id)
        ///              ,()=>new BLL.S_MenuList().Fetch()
        ///              , TimeSpan.FromMinutes(60)
        ///             );
        /// </remarks>
        /// <returns></returns>
        public static T GetCache<T>(string CacheKey, Func<T> cachePopulate, TimeSpan slidingExpiration)
        {
            if (String.IsNullOrWhiteSpace(CacheKey))
            {
                throw new ArgumentException("无效的缓存键");
            }
            if (cachePopulate == null)
            {
                throw new ArgumentNullException("缓存填充");
            }
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            object objObject = objCache[CacheKey];
            if (objObject == null)
            {
                lock (_locker)
                {
                    var item = new System.Runtime.Caching.CacheItem(CacheKey, cachePopulate());
                    SetCache(CacheKey, item.Value, slidingExpiration);
                }

            }
            return (T)objCache[CacheKey];
        }

        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey">键</param>
        /// lamda表达式:可以是() => new BLL.CangFangList().Single(CangFangId)或者() => "abc"
        /// 或者委托(注意返回)： delegate() { return new BLL.CangFangList().Single(CangFangId); }
        /// <param name="slidingExpiration">用于设置可调过期时间，它表示当离最后访问超过某个时间段后就过期new TimeSpan(0, 10, 0)或者TimeSpan.FromMinutes(60)</param>
        /// <param name="absoluteExpiration">用于设置绝对过期时间,System.DateTime.Now.AddMinutes(20)</param>
        /// <returns></returns>
        public static T GetCache<T>(string CacheKey, Func<T> cachePopulate, TimeSpan slidingExpiration, DateTime absoluteExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            object objObject = objCache[CacheKey];
            if (objObject == null)
            {
                var item = new System.Runtime.Caching.CacheItem(CacheKey, cachePopulate());
                SetCache(CacheKey, item, absoluteExpiration, slidingExpiration);
            }
            return (T)objCache[CacheKey];
        } 
        #endregion


        #region 设置数据缓存
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject, TimeSpan Timeout)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, DateTime.MaxValue, Timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        } 
        #endregion


        #region 移除指定数据缓存
        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void RemoveAllCache(string CacheKey)
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            _cache.Remove(CacheKey);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        } 
        #endregion
    }
}