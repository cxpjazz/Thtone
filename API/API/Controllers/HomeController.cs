using Senparc.Weixin;
using Senparc.Weixin.WxOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API.Models;

namespace API.Controllers
{
    public class HomeController : BaseController
    {
        Models.DBDataContext db = new Models.DBDataContext(con);


        #region 微信登录
        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnLogin(string code)
        {
            ///获取集合
            var jsonresult = Senparc.Weixin.WxOpen.AdvancedAPIs.Sns.SnsApi.JsCode2Json(Appid, AppSecret, code);
            if (jsonresult.errcode == ReturnCode.请求成功)
            {
                int count = db.UserAccount.Count(o => o.openid == jsonresult.openid);
                if (count<1)
                {
                    UserAccount accout = new UserAccount();
                    accout.openid = jsonresult.openid;
                    accout.CreateTime = DateTime.Now;
                    db.UserAccount.InsertOnSubmit(accout);
                    db.SubmitChanges();
                }
                ///生成一个sessionid
                string sessionId = Guid.NewGuid().ToString().Substring(0, 16);
                ///openid存入cookie
                CoreHelper.CookieHelper.AddCookies(sessionId, CoreHelper.Encrypt.AES.AesEncrypt(jsonresult.openid, AES));

                return Json(new { d = "ok", value = sessionId });
            }
            else
            {
                return Json(new { success = false, msg = jsonresult.errmsg });
            }



        }
        
        #endregion


        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult test()
        {
            
            return Json(new { sucess=true,msg="么么哒"});

        }



    }
}
