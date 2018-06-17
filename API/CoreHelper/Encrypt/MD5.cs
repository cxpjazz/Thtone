using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoreHelper.Encrypt
{
    public class MD5
    {
        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="instr">要加密的字符串</param>
        /// <param name="enc">编码方式</param>
        /// <returns></returns>
        public static string EncryptMD5(string instr, Encoding enc = null)
        {
            string result;
            try
            {
                if (enc == null)
                {
                    enc = Encoding.Default;
                }
                byte[] toByte = enc.GetBytes(instr);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                toByte = md5.ComputeHash(toByte);
                result = BitConverter.ToString(toByte).Replace("-", "");
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        } 
        #endregion
    }
}
