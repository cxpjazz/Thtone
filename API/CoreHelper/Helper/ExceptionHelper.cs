using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace CoreHelper
{
    public class ExceptionHelper
    {

		public static Exception GetInnerException(Exception exp)
		{
			if (exp.InnerException != null)
			{
				exp = exp.InnerException;
				return GetInnerException(exp);
			}
			return exp;
		}
        static long exceptionId
        {
            get
            {
                return CoreConfig.Instance.LogMsgId;
            }
            set
            {
                CoreConfig.Instance.LogMsgId = value;
            }
        }
		static object lockObj = new object();		
        /// <summary>
        /// 内部记录日志
        /// </summary>
        /// <param name="ero"></param>
        /// <returns></returns>
        public static string InnerLogException(Exception ero)
        {
            string host = HttpContext.Current.Request.Url.Host.ToUpper();
            string errorCode = host.Replace(".", "_");
            lock (lockObj)
            {
                exceptionId += 1;
                errorCode += ":" + EventLog.GetSecondFolder() + ":" + exceptionId;
            }

            ero = GetInnerException(ero);
            EventLog.LogItem item = new EventLog.LogItem();
            item.Title = "页面发生错误,错误代码:" + errorCode;
            if (ero != null)
            {
                item.Detail = ero.Message + ":" + ero.StackTrace;
            }
            CoreHelper.EventLog.Log(item,"Error");
            if (host == "LOCALHOST")
            {
                return errorCode;
            }
            return errorCode;
        }
       
    }
}
