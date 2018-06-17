using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
namespace CoreHelper
{
	public class HttpRequest
	{
        /// <summary>
        /// http post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="enc"></param>
        /// <param name="contentType"></param>
        /// <param name="proxyHost">代理</param>
        /// <returns></returns>
        public static string HttpPost(string url, string data, Encoding enc, string contentType = "application/x-www-form-urlencoded", string proxyHost="")
		{
			string str;

			HttpWebRequest myHttpWebRequest;// = (HttpWebRequest)WebRequest.Create(url);

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                myHttpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                myHttpWebRequest.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                myHttpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            }

            if (!string.IsNullOrEmpty(proxyHost))
            {
                System.Net.WebProxy proxy = new WebProxy(proxyHost);
                myHttpWebRequest.Proxy = proxy;
            }
            else
            {
                myHttpWebRequest.Proxy = WebRequest.GetSystemWebProxy();
            }
			myHttpWebRequest.Method = "POST";

			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] byte1 = encoding.GetBytes(data);
			//myHttpWebRequest.Timeout=10;

            myHttpWebRequest.ContentType = contentType;

			myHttpWebRequest.ContentLength = data.Length;
			Stream newStream = myHttpWebRequest.GetRequestStream();
			newStream.Write(byte1, 0, byte1.Length);

			// Close the Stream object.
			newStream.Close();
			HttpWebResponse response = myHttpWebRequest.GetResponse() as HttpWebResponse;
			//Encoding enc = Encoding.GetEncoding(encode);
			// now we want to get the page contents of the target body
			using (StreamReader requestReader = new StreamReader(response.GetResponseStream(), enc))
			{
				str = requestReader.ReadToEnd();
			}
			response.Close();

			return str;


		}
        private static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true; //总是接受  
        } 
        /// <summary>
        /// 指定编码GET
        /// </summary>
        /// <param name="url"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string HttpGet(string url, Encoding enc)
        {
            return HttpGet(url, null, enc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="proxyHost">代理地址</param>
        /// <param name="enc"></param>
        /// <returns></returns>
		public static string HttpGet(string url,string proxyHost, Encoding enc)
		{
			string strResult;

			HttpWebRequest myReq;// = (HttpWebRequest)HttpWebRequest.Create(url);
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                myReq = WebRequest.Create(url) as HttpWebRequest;
                myReq.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                myReq = WebRequest.Create(url) as HttpWebRequest;
            }
            myReq.UseDefaultCredentials = true;
			myReq.Timeout = 15000;
            if (!string.IsNullOrEmpty(proxyHost))
            {
                System.Net.WebProxy proxy = new WebProxy(proxyHost);
                myReq.Proxy = proxy;
            }
            else
            {
                myReq.Proxy = WebRequest.GetSystemWebProxy();
            }
			HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
			Stream myStream = HttpWResp.GetResponseStream();
			StreamReader sr = new StreamReader(myStream, enc);
            strResult = sr.ReadToEnd();
			sr.Close();
            myStream.Close();
			return strResult;
		}
	}
}
