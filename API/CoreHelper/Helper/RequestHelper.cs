using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Reflection;
using System.Collections.Specialized;

namespace CoreHelper
{
    public class RequestHelper
    {
        /// <summary>
        /// 是否为远程服务器,DEBUG用
        /// </summary>
        public static bool IsRemote
        {
            get
            {
                string address = CoreHelper.RequestHelper.GetServerIp();
                bool a = !address.Contains("192.168.") && !address.Contains("127.0.") && !address.Contains("10.0.");
                return a;
            }
        }
        /// <summary>
        /// 是否为公司IP或内网请求
        /// </summary>
        public static bool IsSaveIp
        {
            get
            {
                string address = GetCdnIP();
                return address == "1.192.126.80" || !IsRemote;
            }
        }
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty || !StringHelper.IsIP(result))
            {
                return "0.0.0.0";
            }

            return result;

        }
		/// <summary>
		/// 获取CDN转发时客户端原始IP
		/// </summary>
		/// <returns></returns>
		public static string GetCdnIP()
		{
			string address = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];

			if (string.IsNullOrEmpty(address))
			{
				address = HttpContext.Current.Request.UserHostAddress.ToString();
			}
			return address;
		}
        static string serverIp;
		/// <summary>
		/// 获取服务器第一个IP
		/// </summary>
		/// <returns></returns>
		public static string GetServerIp()
		{
            if (string.IsNullOrEmpty(serverIp))
            {
                System.Net.IPAddress[] addressList = Dns.GetHostAddresses(Dns.GetHostName());

                string address = "";
                foreach (System.Net.IPAddress a in addressList)
                {
                    if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !a.ToString().Contains("10.0.0"))
                    {
                        address = a.ToString();
                        break;
                    }
                }
                serverIp = address;
            }
            return serverIp;
		}
        static object lockObj = new object();
        static int index = 1;

        /// <summary>
        /// 设置不同类型SoapHead的值
        /// </summary>
        /// <param name="_head"></param>
        /// <param name="key"></param>
        public static void SetSoapHead(object _head,string key)
        {
            string User = System.Guid.NewGuid().ToString();
            lock (lockObj)
            {
                index += 1;
                User += index.ToString();
            }

            long Time = DateTime.Now.ToBinary();
            //string key = CustomSetting.GetConfigKey("SIGN_KEY");
            string Hash = Encrypt.MD5.EncryptMD5(User + key + Time);

            Type type = _head.GetType();

            PropertyInfo property = type.GetProperty("User");
            property.SetValue(_head, User, null);

            property = type.GetProperty("Hash");
            property.SetValue(_head, Hash, null);

            property = type.GetProperty("Time");

            property.SetValue(_head, Time, null);
        }
        /// <summary>
        /// 返回当前http主机名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHost()
        {
            string url = HttpContext.Current.Request.Url.ToString();
            return GetCurrentHost(url);
        }
        /// <summary>
        /// 返回当前http主机名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHost(string url)
        {
            string[] arry = url.Split('/');
            string host = arry[2];
            string url1 = arry[0] + "//" + host;
            return url1;
        }
        #region 签名机制
        static string timeFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 生成签名参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parmes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetParameUrl(string key, SortedDictionary<string, string> parmes, Encoding encoding)
        {
            string par = "";
            DateTime time = DateTime.Now;
            parmes.Add("time", time.ToString(timeFormat));

            par = GetParame(parmes, true, encoding);
            string par1 = GetParame(parmes, false, encoding);
            string sign = Encrypt.MD5.EncryptMD5(par1 + key);
            par += "&sign=" + sign;
            return par;
        }
        static string GetParame(SortedDictionary<string, string> parmes, bool urlEncode, Encoding encoding)
        {
            string par = "";
            foreach (KeyValuePair<string, string> entry in parmes)
            {
                if (entry.Key != "sign")
                {
                    if (urlEncode)
                    {
                        par += entry.Key.ToLower() + "=" + HttpUtility.UrlEncode(entry.Value, encoding) + "&";
                    }
                    else
                    {
                        par += entry.Key.ToLower() + "=" + entry.Value + "&";
                    }
                }
            }
            par = par.Substring(0, par.Length - 1);
            return par;
        }
        /// <summary>
        /// 验证数据
        /// </summary>
        public static bool VerifyData(string key, SortedDictionary<string, string> parmes, out string msg, Encoding encoding)
        {
            msg = "";
            string time = parmes["time"];
            string sign = parmes["sign"];
            TimeSpan ts = DateTime.Now - Convert.ToDateTime(time);
            if (ts.TotalMinutes > 10)
            {
                msg = "签名超时";
                return false;
            }
            string par = GetParame(parmes, false, encoding);
            string sign1 = Encrypt.MD5.EncryptMD5(par + key);
            if (sign != sign1)
            {
                //CoreHelper.EventLog.Log("签名为:" + par,false);
                msg = "签名验证失败";
                return false;
            }
            return true;
        }
        /// <summary>
        /// 从集合中获取指定的参数
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetRequestParame(NameValueCollection cc, string pars)
        {
            SortedDictionary<string, string> list = new SortedDictionary<string, string>();
            string[] arry = pars.Split(',');
            foreach (string s in arry)
            {
                if (s.Trim() != "")
                {
                    list.Add(s, cc[s]);
                }
            }
            list.Add("time", cc["time"]);
            list.Add("sign", cc["sign"]);
            return list;
        }
        #endregion
    }
}
