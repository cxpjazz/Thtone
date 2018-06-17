using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace API.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 获取小程序的appid
        /// </summary>
        public readonly static string Appid = ConfigurationManager.AppSettings["Appid"];

        /// <summary>
        /// 小程序的密钥
        /// </summary>
        public readonly static string AppSecret = ConfigurationManager.AppSettings["AppSecret"];


        public readonly static string AES = "wtfUfLPUwzWnG2aA";

        public readonly static string con = CoreHelper.Encrypt.DES2.Decrypt(ConfigurationManager.ConnectionStrings["minapiConnectionString"].ConnectionString);


        public string GetOpenid(string sessionId)
        {
            string openid = CoreHelper.CookieHelper.GetCookieValue(sessionId);
            openid = CoreHelper.Encrypt.AES.AesDecrypt(openid, AES);
            return openid;
        }





    }
}
