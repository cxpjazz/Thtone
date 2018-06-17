using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CoreHelper.Encrypt
{
    /// <summary>
    /// 对应java中的DES算法
    /// </summary>
    public class DES2
    {
        //对称加密密钥
        private static byte[] key_192 = { (byte)29, 33, (byte)62, (byte)138, (byte)217, (byte)198, 53, 27 };
        //对称加密初始化向量
        private static byte[] iv_192 = { 89, 32, (byte)210, 63, 20, (byte)135, 26, (byte)236 };

        #region 使用对称加密算法加密字符串，使用默认密钥
        /// <summary>
        /// 使用对称加密算法加密字符串，使用默认密钥
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encrypt(string str)
        {
            str = System.Web.HttpUtility.UrlEncode(str);
            if (string.IsNullOrEmpty(str)) return "";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            des.Key = key_192;
            des.IV = iv_192;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        } 
        #endregion

        #region 使用对称加密算法解密字符串，使用默认密钥
        /// <summary>
        /// 使用对称加密算法解密字符串，使用默认密钥
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decrypt(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return "";
                //str = str;
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[str.Length / 2];
                for (int x = 0; x < str.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(str.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = key_192;
                des.IV = iv_192;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return System.Web.HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
            }
            catch
            {
                return "";
            }
        } 
        #endregion


        #region 使用对称加密算法加密字符串，自定义密钥
        /// <summary>
        /// 使用对称加密算法加密字符串，可设置密钥
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encrypt(string str, byte[] key_192, byte[] iv_192)
        {
            str = System.Web.HttpUtility.UrlEncode(str);
            if (string.IsNullOrEmpty(str)) return "";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            des.Key = key_192;
            des.IV = iv_192;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }
        #endregion

        #region 使用对称加密算法解密字符串，自定义密钥
        /// <summary>
        /// 使用对称加密算法解密字符串，可设置密钥
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decrypt(string str, byte[] key_192, byte[] iv_192)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return "";
                //str = str;
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[str.Length / 2];
                for (int x = 0; x < str.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(str.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = key_192;
                des.IV = iv_192;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return System.Web.HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
            }
            catch
            {
                return "";
            }
        }
        #endregion


        #region 使用对称加密算法加密字符串,使用普通字符串作为密钥
        /// <summary>
        /// 使用对称加密算法加密字符串,使用普通字符串作为密钥
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string sourceData, string key)
        {
            Byte[] keys = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, key.Length));
            Byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var data = Encrypt(sourceData, keys, iv);
            return data;
        } 
        #endregion

        #region 使用对称加密算法解密字符串,使用普通字符串作为密钥
        /// <summary>
        /// 使用对称加密算法解密字符串,使用普通字符串作为密钥
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string sourceData, string key)
        {
            Byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            Byte[] keys = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, key.Length));
            var data2 = Decrypt(sourceData, keys, iv);
            return data2;
        } 
        #endregion


        #region 加密，只要key，不需要IV和填充，与python语言通信
        /// <summary>
        /// 加密，只要key，不需要IV和填充，与python语言通信
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptPython(string message, string key)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(message);
                des.Key = UTF8Encoding.UTF8.GetBytes(key);
                //des.IV = UTF8Encoding.UTF8.GetBytes(key);
                des.Mode = System.Security.Cryptography.CipherMode.ECB;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                //string str = Convert.FromBase64String(ms.ToArray());
                string str = StringHelper.ByteToHex(ms.ToArray());
                ms.Close();
                return str;
            }
        } 
        #endregion

        #region 解密，只要key，不需要IV和填充，与python语言通信
        /// <summary>
        /// 解密，只要key，不需要IV和填充，与python语言通信
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptPython(string message, string key)
        {

            byte[] inputByteArray = Convert.FromBase64String(StringHelper.HexToBase64String(message));
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = UTF8Encoding.UTF8.GetBytes(key);
                //des.IV = UTF8Encoding.UTF8.GetBytes(key);
                des.Mode = System.Security.Cryptography.CipherMode.ECB;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }        
        #endregion


    }
}
