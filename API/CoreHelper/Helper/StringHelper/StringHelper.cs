using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Reflection;


namespace CoreHelper
{
    /// <summary>
    /// 字符串转换
    /// </summary>
    public partial class StringHelper
    {
        #region BYTE转16进制
        /// <summary>
        /// BYTE转16进制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteToHex(byte[] data)
        {
            string returnStr = "";
            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    returnStr += data[i].ToString("X2");
                }
            }
            return returnStr;
        }
        #endregion

        #region 16进制转BYTE
        /// <summary>
        /// 16进制转BYTE
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }
        #endregion

        #region 16进制转Base64
        /// <summary>
        /// 16进制转Base64
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexToBase64String(string hexString)
        {
            MatchCollection mc = Regex.Matches(hexString.ToString().ToUpper(), "[A-F0-9]{2}");
            byte[] bytes = new byte[mc.Count];
            for (int i = 0; i < mc.Count; i++)
            {
                bytes[i] = byte.Parse(mc[i].Value, System.Globalization.NumberStyles.HexNumber);
            }
            return Convert.ToBase64String(bytes);
        }
        #endregion

        #region Base64转16进制
        /// <summary>
        /// Base64转16进制
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        public static string Base64ToHex(string strBase64)
        {
            char[] values = strBase64.ToCharArray();
            string output = "";
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = String.Format("{0:X}", value);
                output += hexOutput;
            }
            return output;
        }
        #endregion

        #region Base64转普通字符串
        /// <summary>
        /// Base64转普通字符串
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        public static string Base64ToString(string strBase64)
        {
            byte[] c = Convert.FromBase64String(strBase64);
            string output = System.Text.Encoding.Default.GetString(c);  
            return output;
        }
        #endregion

        #region 普通字符串转Base64
        /// <summary>
        /// Base64转普通字符串
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        public static string StringToBase64(string strBase64)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(strBase64);
            //转成 Base64 形式的 System.String  
            string output = Convert.ToBase64String(b);
            return output;
        }
        #endregion

        #region 常规字符转16进制
        public static String strToHex(String str)
        {

            char[] chars = "0123456789ABCDEF".ToCharArray();
            StringBuilder sb = new StringBuilder("");
            byte[] bs = Encoding.UTF8.GetBytes(str);
            int bit;

            for (int i = 0; i < bs.Length; i++)
            {
                bit = (bs[i] & 0x0f0) >> 4;
                sb.Append(chars[bit]);
                bit = bs[i] & 0x0f;
                sb.Append(chars[bit]);
                //sb.Append(' ');
            }
            return sb.ToString().Trim();
        }
        #endregion

        #region 字符串前补0
        /// <summary>
        /// 字符串前补0
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="size">总长度</param>
        /// <returns></returns>
        public static string FillZero(string value, int size)
        {
            string tmp = "";
            for (int i = 0; i < size - value.Length; i++)
            {
                tmp += "0";
            }

            return tmp + value;
        } 
        #endregion
        
        #region 截取字符串
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="number">截取数量</param>
        /// <returns></returns>
        public static string InterceptString(string str, int number)
        {
            if (str == null || str.Length == 0 || number <= 0) return "";
            int iCount = System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(str);
            if (iCount > number)
            {
                int iLength = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    int iCharLength = System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(new char[] { str[i] });
                    iLength += iCharLength;
                    if (iLength == number)
                    {
                        str = str.Substring(0, i + 1);
                        break;
                    }
                    else if (iLength > number)
                    {
                        str = str.Substring(0, i);
                        break;
                    }
                }
            }
            return str;
        } 
        #endregion

        #region 截取字符串，以“...”结束
        /// <summary>
        /// 截取字符串，以“...”结束
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="number">截取数量</param>
        /// <returns></returns>
        public static string InterceptStringEndDot(string str, int number)
        {
            if (str == null || str.Length == 0 || number <= 0) return "";
            int iCount = System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(str);
            if (iCount > number)
            {
                int iLength = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    int iCharLength = System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(new char[] { str[i] });
                    iLength += iCharLength;
                    if (iLength == number)
                    {
                        str = str.Substring(0, i + 1) + "…";
                        break;
                    }
                    else if (iLength > number)
                    {
                        str = str.Substring(0, i) + "…";
                        break;
                    }
                }
            }
            return str;
        } 
        #endregion

        #region 判断字符串是否在几个字符之中
        /// <summary>
        /// 判断字符串是否在几个字符之中
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <param name="strs">几个字符串，就是范围</param>
        /// <returns>如果在返回true，否则返回false</returns>
        public static bool IsIn(string str, params string[] strs)
        {
            foreach (string s in strs)
            {
                if (s == str)
                    return true;
            }
            return false;
        } 
        #endregion

        #region 得到随机数，包含数字，字母大小写
        /// <summary>
        /// 得到随机数，包含数字，字母大小写
        /// </summary>
        /// <param name="count">个数</param>
        /// <returns></returns>
        public static string GetCheckCode(int count)
        {
            char[] character = { '1', '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            Random rnd = new Random();
            //生成验证码字符串
            string chkCode = string.Empty;
            for (int i = 0; i < count; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            return chkCode;
        } 
        #endregion

        #region 得到随机数,纯数字
        /// <summary>
        /// 得到随机数,纯数字
        /// </summary>
        /// <param name="count">个数</param>
        /// <returns></returns>
        public static string GetCheckCodeNum(int count)
        {
            char[] character = { '1', '2', '3', '4', '5', '6', '8', '9', '0' };
            Random rnd = new Random();
            //生成验证码字符串
            string chkCode = string.Empty;
            for (int i = 0; i < count; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            return chkCode;
        } 
        #endregion

        #region 得到0-10的汉字显示
        /// <summary>
        /// 得到0-10的汉字显示
        /// </summary>
        /// <param name="number">数字</param>
        /// <param name="isTraditional">是否是繁体</param>
        /// <returns></returns>
        public static string GetChineseNumber(int number, bool isTraditional=false)
        {
            var str = number.ToString();
            string str2 = "";
            foreach (var s in str)
            {
                str2 += GetChineseNumber(s.ToString(), isTraditional);
            }
            return str2;
        } 
        #endregion

        #region 得到0-10的汉字显示
        /// <summary>
        /// 得到0-10的汉字显示
        /// </summary>
        /// <param name="number">数字</param>
        /// <param name="isTraditional">是否是繁体</param>
        /// <returns></returns>
        public static string GetChineseNumber(string number, bool isTraditional)
        {
            switch (number)
            {
                case "1":
                    return isTraditional ? "壹" : "一";
                case "2":
                    return isTraditional ? "贰" : "二";
                case "3":
                    return isTraditional ? "叁" : "三";
                case "4":
                    return isTraditional ? "肆" : "四";
                case "5":
                    return isTraditional ? "伍" : "五";
                case "6":
                    return isTraditional ? "陆" : "六";
                case "7":
                    return isTraditional ? "柒" : "七";
                case "8":
                    return isTraditional ? "捌" : "八";
                case "9":
                    return isTraditional ? "玖" : "九";
                case "10":
                    return isTraditional ? "拾" : "十";
                case "0":
                    return isTraditional ? "零" : "〇";
                default:
                    return string.Empty;
            }
        } 
        #endregion

    }
}
