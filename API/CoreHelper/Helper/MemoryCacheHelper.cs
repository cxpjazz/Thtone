using System;
using System.Runtime.Caching;

namespace CoreHelper
{
    /// <summary>
    /// 基于MemoryCache的缓存辅助类,用于Winform桌面程序
    /// </summary>
    public static class MemoryCacheHelper
    {
        private static readonly Object _locker = new object();

        /*
         * slidingExpiration：用于设置可调过期时间，它表示当离最后访问超过某个时间段后就过期，所以类型为System.TimeSpan，当给这个参数设置了一个时间段时，absoluteExpiration的值就只能为Cache.NoAbsoluteExpiration，否则出错；
         * absoluteExpiration：用于设置绝对过期时间，它表示只要时间一到就过期，所以类型为System.DateTime，当给这个参数设置了一个时间时，slidingExpiration参数的值就只能为Cache.NoSlidingExpiration，否则出错；
         * 当然，也允许这两个参数都不设置值，那么absoluteExpiration值为Cache.NoAbsoluteExpiration，slidingExpiration值为Cache.NoSlidingExpiration也是可以的，比如缓存依赖于某个文件时，这就非常有用。
         * 调用方法1：
         * var 缓存对象 = CoreHelper.MemoryCacheHelper.GetCacheItem<Model.CangFangList>("缓存名称", () => new BLL.CangFangList().Single(CangFangId), new TimeSpan(0, 1, 0));
         * 调用方法2：
         * var 缓存对象 = CoreHelper.MemoryCacheHelper.GetCacheItem<Model.CangFangList>("缓存名称", delegate() { return new BLL.CangFangList().Single(CangFangId); }, new TimeSpan(0, 1, 0));
         */
        /// <summary>
        /// 缓存类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存名称</param>
        /// <param name="cachePopulate">
        /// lamda表达式:可以是() => new BLL.CangFangList().Single(CangFangId)或者() => "abc"
        /// 或者委托(注意返回)： delegate() { return new BLL.CangFangList().Single(CangFangId); }
        /// </param>
        /// <param name="slidingExpiration">用于设置可调过期时间，它表示当离最后访问超过某个时间段后就过期new TimeSpan(0, 10, 0)或者TimeSpan.FromMinutes(60)</param>
        /// <param name="absoluteExpiration">用于设置绝对过期时间,System.DateTime.Now.AddMinutes(20)</param>
        /// <returns></returns>
        public static T GetCacheItem<T>(String key, Func<T> cachePopulate, TimeSpan? slidingExpiration = null, DateTime? absoluteExpiration = null)
        {
            
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("无效的缓存键");
            if (cachePopulate == null) throw new ArgumentNullException("缓存填充");
            //强制需要设置一个值
            if (slidingExpiration == null && absoluteExpiration == null) throw new ArgumentException("slidingExpiration或absoluteExpiration参数必须提供一个");

            if (MemoryCache.Default[key] == null)
            {
                lock (_locker)
                {
                    var item = new CacheItem(key, cachePopulate());
                    var policy = CreatePolicy(slidingExpiration, absoluteExpiration);

                    MemoryCache.Default.Add(item, policy);
                }
            }

            return (T)MemoryCache.Default[key];
        }

        private static CacheItemPolicy CreatePolicy(TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            var policy = new CacheItemPolicy();

            if (absoluteExpiration.HasValue)
            {
                policy.AbsoluteExpiration = absoluteExpiration.Value;
            }
            else if (slidingExpiration.HasValue)
            {
                policy.SlidingExpiration = slidingExpiration.Value;
            }

            policy.Priority = CacheItemPriority.Default;

            return policy;
        }
    }
}