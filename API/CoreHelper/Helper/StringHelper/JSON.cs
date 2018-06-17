using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.InteropServices;
using System.IO;
namespace CoreHelper
{
    public partial class StringHelper
    {
        #region ToJson
        /// <summary>
        /// 根据DataTable生成json对象数组格式
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static string ToJSON(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            
            sb.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                sb.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.AppendFormat("\"{0}\":",dt.Columns[j].ColumnName);
                    string strValue = getJSONValue(drc[i][j]);

                    if (j == dt.Columns.Count - 1)
                    {
                        sb.Append(strValue);
                    }
                    else
                    {
                        sb.AppendFormat("{0},", strValue);
                    }
                }
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 根据DataRow生成json对象格式
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <returns></returns>
        public static string ToJSON(DataRow dr)
        {
            DataColumnCollection dcc = dr.Table.Columns;
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < dcc.Count; i++)
            {
                sb.AppendFormat("\"{0}\":", dcc[i].ColumnName);
                string strValue = getJSONValue(dr[i]);
                if (i == dcc.Count - 1)
                {
                    sb.Append(strValue);
                }
                else
                {
                    sb.AppendFormat("{0},", strValue);
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 根据对象生成json对象格式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ToJSON(object obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            Type t = obj.GetType();
            PropertyInfo[] ps=t.GetProperties();
            for(int i=0;i<ps.Length ;i++)
            {
                if (i == ps.Length-1)
                {
                    sb.AppendFormat("\"{0}\":{1}", ps[i].Name, getPropertyValue(obj, ps[i]));
                }
                else
                {
                    sb.AppendFormat("\"{0}\":{1},", ps[i].Name, getPropertyValue(obj, ps[i]));
                }
            }
            
            sb.Append("}");
            return sb.ToString();
        }
        /// <summary>
        /// 根据对象集合生成json对象数组格式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="list">对象集合</param>
        /// <returns></returns>
        public static string ToJSON<T>(IList<T> list)
        {
            if (list.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            string name= typeof (T).Name ;
            sb.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(ToJSON(list[i]));
                }
                else
                {
                    sb.AppendFormat("{0},", ToJSON(list[i]));
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        private static string getPropertyValue(object obj, PropertyInfo pinfo)
        {
            object value = pinfo.GetValue(obj, null);
            if (value == null)
            {
                return "null";
            }
            return getJSONValue(value);
        }
        private static string getJSONValue(object value)
        {
            //if (value is string || value is DateTime || value is Guid || value is TimeSpan)
            //{
                return "\"" + value.ToString().Replace("\"", "\\\"") + "\"";
            //}
            //else if (value is bool)
            //{
            //    return value.ToString().ToLower();
            //}
            //else
            //{
            //    return value.ToString();
            //}
        }
        /// <summary>
        /// 把对象序列化成JSON,支持层级
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SerializerToJson(object source)
        {
            return SerializeHelper.SerializerToJson(source);
        }
        /// <summary>
        /// 把JSON串转换成的BYTE转换成对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object SerializerFromJSON(byte[] data, Type type)
        {
            return SerializeHelper.SerializerFromJSON(data, type);
        }
        #endregion
    }
}
