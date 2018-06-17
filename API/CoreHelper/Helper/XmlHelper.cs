using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;


/*
    tongzhaozhe 
 *  2011-11-4
 */
namespace CoreHelper
{
    /// <summary>
    /// xml文档类型 0 xml文件 1 xml值
    /// </summary>
    public enum XmlType
    {
        File=0,
        Value=1
    }
    public class XmlHelper
    {
        #region 属性

        private static XmlDocument _doc;

        #endregion

        #region 构造函数
        static XmlHelper()
        {
            _doc = new XmlDocument();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 根据xml文档类型和节点路径获取xml节点
        /// </summary>
        /// <param name="value">节点路径 例:/root/username</param>
        /// <param name="xmlValue">xml文件名或者xml文档</param>
        /// <param name="type">xml文档类型 参照XmlType</param>
        /// <returns></returns>
        public static XmlNode GetNode(string value, string xmlValue,XmlType type)
        {
            try
            {
                if (type == XmlType.File)
                    _doc.Load(xmlValue);
                else if (type == XmlType.Value)
                    _doc.LoadXml(xmlValue);
                else
                    throw new Exception("未知xml类型");
                XmlNode node = _doc.SelectSingleNode(value);
                if (node != null)
                {
                    return node;
                }
                else
                {
                    throw new Exception("该节点不存在!");
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        /// <summary>
        /// 通过xml内容路径获取xml节点列表
        /// </summary>
        /// <param name="value">节点路径 例:/root/username</param>
        /// <param name="xmlValue">xml文件名或者xml文档</param>
        /// <param name="type">xml文档类型 参照XmlType</param>
        /// <returns></returns>
        public static XmlNodeList GetNodeList(string value, string xmlValue, XmlType type)
        {
            try
            {
                if (type == XmlType.File)
                    _doc.Load(xmlValue);
                else if (type == XmlType.Value)
                    _doc.LoadXml(xmlValue);
                else
                    throw new Exception("未知xml类型");
                XmlNodeList nodeList = _doc.SelectNodes(value);
                if (nodeList != null)
                {
                    return nodeList;
                }
                else
                {
                    throw new Exception("该节点不存在!");
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        /// <summary>
        /// 通过xml文件路径获取某个节点的值
        /// </summary>
        /// <param name="path">节点路径 例:/root/username</param>
        /// <param name="xmlValue">xml文件名或者xml文档</param>
        /// <param name="type">xml文档类型 参照XmlType</param>
        /// <returns></returns>
        public static string GetNodeValue(string path, string xmlValue,XmlType type)
        {
            string value = string.Empty;
            try
            {
                XmlNode node = GetNode(path, xmlValue, type);
                value = node.InnerText;
            }
            catch (Exception ee)
            {
                value = "错误:" + ee.Message;
            }
            return value;
        }
        /// <summary>
        /// 通过xml内容获取某个节点的某个属性的值
        /// </summary>
        /// <param name="path">节点路径 例:/root/username</param>
        /// <param name="attribute">属性名</param>
        /// <param name="xmlValue">xml文件名或者xml文档</param>
        /// <param name="type">xml文档类型 参照XmlType</param>
        /// <returns></returns>
        public static string GetAttributeValue(string path, string attribute, string xmlValue,XmlType type)
        {
            string value = string.Empty;
            try
            {
                XmlNode node = GetNode(path, xmlValue, type);
                XmlAttribute xmlAttribute = node.Attributes[attribute];
                if (xmlAttribute != null)
                {
                    value = xmlAttribute.Value;
                }
                else
                {
                    value = "错误:该节点的该属性不存在!";
                }
            }
            catch (Exception ee)
            {
                value = "错误:" + ee.Message;
            }
            return value;
        }
        /// <summary>
        /// 通过xml路径读取xml内容至DataSet中
        /// </summary>
        /// <param name="strXmlPath">xml文档路径</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string strXmlPath)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(System.Web.HttpContext.Current.Server.MapPath(strXmlPath));
                
                if (ds.Tables.Count > 0)
                {
                    return ds;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 通过临时XmlValue加载至DataTable中，自动生成DataTable架构
        /// </summary>
        /// <param name="strXmlValue">xml文档</param>
        /// <param name="xpath">节点路径 例:/root/username</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string strXmlValue,string xpath)
        {
            DataTable tb = new DataTable();
            try
            {
                XmlNodeList nodeList = GetNodeList(xpath, strXmlValue, XmlType.Value);
                XmlNode firstNode = nodeList.Item(0);
                foreach (XmlNode firstNode_sub in firstNode.ChildNodes)
                {
                    tb.Columns.Add(firstNode_sub.Name,typeof(string));
                }
                foreach (XmlNode node in nodeList)
                {
                    XmlNodeList itemList = node.ChildNodes;
                    DataRow row = tb.NewRow();
                    foreach (XmlNode itemNode in itemList)
                    {
                        row[itemNode.Name] = itemNode.InnerText;
                    }
                    tb.Rows.Add(row);
                }
                return tb;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 插入包含属性的节点
        /// </summary>
        /// <param name="xmlValue">xml文件名或者xml文档</param>
        /// <param name="xpath">节点路径 例:/root/username</param>
        /// <param name="element">节点名称</param>
        /// <param name="value">节点值</param>
        /// <param name="attribute">属性名称</param>
        /// <param name="attValue">属性值</param>
        /// <param name="type">xml文档类型 参照XmlType</param>
        public static void InsertNode(string xmlPath, string xpath, string element, string value,string attribute,string attValue)
        {
            if (element.Equals("") || element.Equals(null))
            {
                throw new Exception("节点名称不能为空");
            }
            XmlNode node = GetNode(xpath, xmlPath, XmlType.File);
            XmlElement xe = _doc.CreateElement(element);
            xe.InnerText = value;
            if (!attribute.Equals("") && !attribute.Equals(null))
            {
                xe.SetAttribute(attribute, attValue);
            }
            node.AppendChild(xe);
            _doc.Save(xmlPath);
        }
        /// <summary>
        /// 为节点插入属性
        /// </summary>
        /// <param name="xmlValue">xml文件名或者xml文档</param>
        /// <param name="xpath">节点路径 例:/root/username</param>
        /// <param name="element">节点名称</param>
        /// <param name="attribute">属性名称</param>
        /// <param name="attValue">属性值</param>
        /// <param name="type">xml文档类型 参照XmlType</param>
        public static void InsertAttribute(string xmlPath, string xpath, string element, string attribute, string attValue)
        {
            if (attribute.Equals("") && attribute.Equals(null))
            {
                throw new Exception("属性名称不能为空");
            }
            XmlNode node = GetNode(xpath, xmlPath, XmlType.File);
            if (node.Attributes[attribute] != null)
            {
                throw new Exception("名称为\"" + attribute + "\"的属性已存在");
            }
            XmlElement xe = (XmlElement)node;
            xe.SetAttribute(attribute, attValue);
            _doc.Save(xmlPath);
        }
        /// <summary>
        /// 更新节点值
        /// </summary>
        /// <param name="xmlPath">xml文件路径</param>
        /// <param name="xpath">节点路径 例:/root/username</param>
        /// <param name="element">节点名称</param>
        /// <param name="value">节点值</param>
        public static void UpdateNode(string xmlPath, string xpath, string element, string value)
        {
            if (element.Equals("") || element.Equals(null))
            {
                throw new Exception("节点名称不能为空");
            }
            XmlNode node = GetNode(xpath, xmlPath, XmlType.File);
            node.InnerText = value;
            _doc.Save(xmlPath);
        }
        /// <summary>
        /// 更新节点属性值
        /// </summary>
        /// <param name="xmlPath">xml文件路径</param>
        /// <param name="xpath">节点路径 例:/root/username</param>
        /// <param name="element">节点名称</param>
        /// <param name="attribute">属性名称</param>
        /// <param name="attValue">属性值</param>
        public static void UpdateAttribute(string xmlPath, string xpath, string element, string attribute, string attValue)
        {
            if (attribute.Equals("") && attribute.Equals(null))
            {
                throw new Exception("属性名称不能为空");
            }
            XmlNode node = GetNode(xpath, xmlPath, XmlType.File);
            if (node.Attributes[attribute] == null)
            {
                throw new Exception("名称为\"" + attribute + "\"的属性不存在");
            }
            XmlElement xe = (XmlElement)node;
            xe.SetAttribute(attribute, attValue);
            _doc.Save(xmlPath);
        }
        /// <summary>
        /// 删除具有某属性的节点
        /// </summary>
        /// <param name="xmlPath">xml文件路径</param>
        /// <param name="xpath">节点路径 例:/root/username</param>
        /// <param name="element">节点名称</param>
        /// <param name="attribute">属性名称</param>
        /// <param name="attValue">属性值</param>
        public static void DeleteNode(string xmlPath, string xpath, string element,string attribute,string attValue)
        {
            if (element.Equals("") || element.Equals(null))
            {
                throw new Exception("节点名称不能为空");
            }
            XmlNode node = GetNode(xpath, xmlPath, XmlType.File);
            foreach (XmlNode s_node in node.ChildNodes)
            {
                XmlElement xe = (XmlElement)s_node;
                if (xe.GetAttribute(attribute) == attValue)
                {
                    xe.RemoveAll();
                    node.RemoveChild(xe);
                    break;
                }
            }
            _doc.Save(xmlPath);
        }
        
        #endregion
    }
}
