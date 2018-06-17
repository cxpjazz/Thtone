using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CoreHelper
{
    /// <summary>
    /// 获取自定义配置值
    /// /CustomSetting.config
    /// 文本 key=value
    /// </summary>
    public class CustomSetting
    {
        public static bool CheckSaveIp(string ip)
        {
            if (!CoreHelper.RequestHelper.IsRemote)
            {
                return true;
            }
            string allowIps = GetConfigKey("LogAllowIps");
            return allowIps.Contains(ip);
        }
        //static Dictionary<string, string> keyCache = new Dictionary<string, string>();
        //const string confgiFile = "/Config/CustomSetting.config";
        /// <summary>
        /// 获取自定义配置值
        /// 如果值用[]包括，则按加密过处理
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigKey(string key)
        {
            string confgiFile = "/Config/CustomSetting.config";
            key = key.ToUpper().Trim();
            var cache = System.Web.HttpRuntime.Cache;
            string configKey = "$CustomSetting";
            var cacheObj = cache.Get(configKey);
            Dictionary<string, string> keyCaches;
            if (cacheObj != null)
            {
                keyCaches = cacheObj as Dictionary<string, string>;
            }
            else
            {
                keyCaches = new Dictionary<string, string>();
                string file = System.Web.Hosting.HostingEnvironment.MapPath(confgiFile);
                if (!System.IO.File.Exists(file))
                {
                    throw new Exception("配置文件不存在:" + confgiFile);
                }
                string content = System.IO.File.ReadAllText(file,Encoding.GetEncoding("gb2312"));
                string[] arry = content.Split('\n');
                foreach (string str in arry)
                {
                    if (str.StartsWith("//"))
                        continue;
                    int index = str.IndexOf("=");
                    if (index == -1)
                        continue;
                    string name = str.Substring(0,index).Trim().ToUpper();
                    string value = str.Substring(index + 1).Trim();
                    try
                    {
                        value = DesString(value);
                        keyCaches.Add(name, value);
                    }
                    catch { }
                }
                cache.Insert(configKey, keyCaches, new System.Web.Caching.CacheDependency(file), DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            if (keyCaches.ContainsKey(key))
                return keyCaches[key];
            throw new Exception("找不到匹配的KEY:" + key + ",或加密错误 请进行配置");
        }
        /// <summary>
        /// DES加密的内容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDESEncrypValue(string value)
        {
            return Encrypt.DES2.Encrypt(value, CoreConfig.EncryptKey);
        }
        /// <summary>
        /// DES解密的内容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetUnDESEncrypValue(string value)
        {
            return Encrypt.DES2.Decrypt(value, CoreConfig.EncryptKey);
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        public static void Clear()
        {
            //keyCache.Clear();
        }
        static string DesString(string value)
        {
            //按DES加密处理
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                try
                {
                    value = Encrypt.DES2.Decrypt(value.Substring(1, value.Length - 2).Trim(), CoreConfig.EncryptKey);
                }
                catch(Exception ero)
                {
                    throw new Exception("解密串时发生错误 ,请检查源文本是否是正确 :" + ero.Message);
                }
            }
            return value;
        }
        
    }
}
