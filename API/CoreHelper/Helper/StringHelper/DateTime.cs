using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreHelper
{
    public partial class StringHelper
    {
        #region 得到日期的汉字显示
        /// <summary>
        /// 得到日期的汉字显示
        /// </summary>
        /// <param name="date">格式为：2011-10-05</param>
        /// <param name="isTraditional">是否繁体字显示</param>
        /// <returns></returns>
        public static string GetChineseDate(string date, bool isTraditional)
        {
            string[] parts = date.Split('-');
            if (parts.Length == 3)
            {
                string nDate = string.Empty;
                foreach (char c in parts[0])
                {
                    nDate += GetChineseNumber(c.ToString(), isTraditional);
                }
                nDate += "年";
                nDate += getDBChinese(parts[1], isTraditional);
                nDate += "月";
                nDate += getDBChinese(parts[2], isTraditional);
                nDate += "日";
                return nDate;
            }
            else
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 得到日期的汉字显示
        /// </summary>
        /// <param name="date">格式为：2011-10-05</param>
        /// <returns></returns>
        public static string GetChineseDate(string date)
        {
            return GetChineseDate(date, false);
        }

        private static string getDBChinese(string number, bool isTraditional)
        {
            if (number.Length == 1)
            {
                return GetChineseNumber(number, isTraditional);
            }

            string nsb = string.Empty;
            string one = number.Substring(0, 1);
            string two = number.Substring(1, 1);
            if (one == "0")
            {
                return GetChineseNumber(two, isTraditional);
            }
            if (one != "1")
            {
                nsb += GetChineseNumber(one, isTraditional);
            }
            nsb += GetChineseNumber("10", isTraditional);
            if (two != "0")
            {
                nsb += GetChineseNumber(two, isTraditional);
            }
            return nsb;
        } 
        #endregion

        #region 时间戳
        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public static string DateTimeToUnix(System.DateTime time)
        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));

            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位  
            return t.ToString();
        } 
        #endregion
    }
}
