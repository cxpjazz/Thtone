using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
namespace CoreHelper
{
	public static class SerializeHelper
	{  
        #region 二进制格式序列化和反序列化

        /// <summary>
        /// 把对象用二进制格式序列化到流
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="stream">目标流</param>
        public static void BinarySerialize(object obj, Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }

        /// <summary>
        /// 把对象用二进制格式序列化到文件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="file">对象的类型</param>
        public static void BinarySerialize(object obj, string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create))
            {
                BinarySerialize(obj, stream);
            }
        }

        /// <summary>
        /// 从流反序列化对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public static T BinaryDeserialize<T>(Stream stream) where T : class
        {
            IFormatter formatter = new BinaryFormatter();
            object obj = formatter.Deserialize(stream);
            if (obj is T)
            {
                return obj as T;
            }
            else
            {
                Type type = typeof(T);
                throw new Exception(string.Format("反序列化后不能得到类型{0}",type.Name));
            }
        }
        
        /// <summary>
        /// 从文件反序列化对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public static T BinaryDeserialize<T>(string file) where T : class
        {
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return BinaryDeserialize<T>(stream);
            }
        }

        #endregion

        #region Soap格式序列化和反序列化
        /// <summary>
        /// 把对象用Soap格式格式序列化到流
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="stream">目标流</param>
        public static void SoapSerialize(object obj, Stream stream)
        {
            IFormatter formatter = new SoapFormatter();
            formatter.Serialize(stream, obj);
        }

        /// <summary>
        /// 把对象用Soap格式格式序列化到文件
        /// </summary>
        /// <param name="obj">流</param>
        /// <param name="file">文件路径</param>
        public static void SoapSerialize(object obj, string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create))
            {
                SoapSerialize(obj, stream);
            }
        }

        /// <summary>
        /// 从流反序列化对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public static T SoapDeserialize<T>(Stream stream) where T:class
        {
            IFormatter formatter = new SoapFormatter();
            object obj = formatter.Deserialize(stream);
            if (obj is T)
            {
                return obj as T;
            }
            else
            {
                Type type = typeof(T);
                throw new Exception(string.Format("反序列化后不能得到类型{1}",type.Name));
            }
        }

        /// <summary>
        /// 从流反序列化对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public static T SoapDeserialize<T>(string file) where T : class
        {
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return SoapDeserialize<T>(stream);
            }
        }

        #endregion 

        #region Xml格式序列化和反序列化

        /// <summary>
        /// 把对象用Xml格式格式序列化到流
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="stream">流</param>
        public static void XmlSerialize(object obj, Stream stream)
        {
            Type type = obj.GetType();
            XmlSerializer xmlSer = new XmlSerializer(type);
            xmlSer.Serialize(stream, obj);
        }

        /// <summary>
        /// 把对象用Xml格式格式序列化到文件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="file">文件</param>
        public static void XmlSerialize(object obj, string file)
        {
            using (Stream stream = new FileStream(file, FileMode.CreateNew, FileAccess.Write))
            {
                XmlSerialize(obj, stream);
            }
        }
        public static string XmlSerialize(object obj, Encoding encode)
        {
            var ms = new System.IO.MemoryStream();
            XmlSerialize(obj, ms);
            string xml = encode.GetString(ms.ToArray());
            ms.Close();
            return xml;
        }
        public static T XmlDeserialize<T>(string xml, Encoding encode) where T : class
        {
            var arry = encode.GetBytes(xml);
            var ms = new System.IO.MemoryStream(arry);
            var obj = CoreHelper.SerializeHelper.XmlDeserialize<T>(ms);
            ms.Close();
            return obj;
        }
        /// <summary>
        /// 从流反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(Stream stream) where T : class
        {
            Type type = typeof(T);
            XmlSerializer xmlSer = new XmlSerializer(type);
            object obj= xmlSer.Deserialize(stream);
            if (obj is T)
            {
                return obj as T;
            }
            else
            {
                throw new Exception(string.Format("反序列化后不能得到类型{0}",type.Name));
            }
        }

        /// <summary>
        /// 从文件反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="file">文件</param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string file) where T : class
        {
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return XmlDeserialize<T>(stream);
            }
        }
        #endregion

        /// <summary>
        /// 利用序列化克隆对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="source">原对象</param>
        /// <returns></returns>
        public static T Clone<T>(T source) where T : class
        {
            if (typeof(T).IsSerializable)
            {
                using (Stream stream = new MemoryStream())
                {
                    BinarySerialize(source, stream);
                    T clone = BinaryDeserialize<T>(stream);
                    return clone;
                }
            }
            else
            {
                throw new Exception(string.Format("不能用序列化的方式克隆类型为{0}的对象",typeof(T).Name));
            }
        }
        /// <summary>
        /// 把对象序列化成JSON,支持层级
        /// 使用UTF8编码,系统显示通用为此编码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SerializerToJson(object source)
        {
            return SerializerToJson(source,System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 把对象序列化成JSON,支持层级
        /// 指定编码,如果要反序列化,则要用相同的编码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string SerializerToJson(object source,System.Text.Encoding enc)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(source);
            //原来方法会序列化background字段
            Type type = source.GetType();
            DataContractJsonSerializer serilializer = new DataContractJsonSerializer(type);
            using (Stream stream = new MemoryStream())
            {
                serilializer.WriteObject(stream, source);
                stream.Flush();
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream, enc);
                string str = reader.ReadToEnd();
                reader.Close();
                return str;
            }
        }
        /// <summary>
        /// 把经过编码转换的JSON转换成对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object SerializerFromJSON(byte[] data, Type type)
        {
            
            using (MemoryStream ms = new MemoryStream(data))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                return serializer.ReadObject(ms);
            }
        }
        /// <summary>
        /// 指定编码转换对象
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static object SerializerFromJSON(string json, Type type,System.Text.Encoding enc)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
            byte[] buffer = enc.GetBytes(json);
            return SerializerFromJSON(buffer,type);
        }


        #region JSON与对象序列化转换
        /// <summary>
        /// 将C#数据实体转化为JSON数据
        /// </summary>
        /// <param name="obj">要转化的数据实体</param>
        /// <returns>JSON格式字符串</returns>
        public static string JsonSerialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            stream.Position = 0;

            StreamReader sr = new StreamReader(stream);
            string resultStr = sr.ReadToEnd();
            sr.Close();
            stream.Close();

            return resultStr;
        }

        /// <summary>
        /// 将JSON数据转化为C#数据实体
        /// </summary>
        /// <param name="json">符合JSON格式的字符串</param>
        /// <returns>T类型的对象</returns>
        public static T JsonDeserialize<T>(string json)
        {

            #region 将时间字符串转成json字符
            //正则表达式,优先匹配带时分秒的时期格式,否则会出错
            string pattern = @"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2})|(\d{4}-\d{2}-\d{2})";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(pattern);
            json = reg.Replace(json, matchEvaluator);
            #endregion


            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json.ToCharArray()));
            T obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
        #endregion


        #region 字符时间与json时间转换
        /// <summary>
        /// 将Json序列化的时间由/Date(1304931520336+0800)/转为字符串
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        } 
        #endregion
    }
}
