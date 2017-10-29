using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Specialized;

namespace XDropsWater.CrossCutting.Xml
{
    public class XmlDocUtil : XmlDocument
    {
        public XmlDocUtil() { }
        /// <summary>
        /// 管理xml数据管理类，继承于XmlDocument
        /// </summary>
        /// <param name="fileName">xml文件名称</param>
        public XmlDocUtil(string fileName)
        {
            Load(fileName);
        }

        /// <summary>
        /// 隐藏基类的方法
        /// </summary>
        /// <param name="fileName"></param>
        public new void Load(string fileName)
        {
            if (!File.Exists(fileName))
                throw new Exception(string.Format("参数fileName文件路径[{0}]不存在！", fileName));

            this.fileName = fileName;
            base.Load(fileName);
        }

        /// <summary>
        /// xml文件路径
        /// </summary>
        private string fileName = string.Empty;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        #region 节点读取功能
        /// <summary>
        /// 获得指定条件节点的子节点集合
        /// </summary>
        /// <param name="NodeName">节点路径</param>
        /// <returns></returns>
        public XmlNodeList GetChildNodes(string NodeName)
        {
            if (string.IsNullOrEmpty(NodeName)) return null;

            XmlNode lTemp_xn = SelectSingleNode(NodeName);
            if (lTemp_xn == null) return null;

            return lTemp_xn.ChildNodes;
        }
        /// <summary>
        /// 获取节点属性
        /// </summary>
        /// <param name="pNodePath_str">Xpath表达式</param>
        /// <param name="pAttributeName_str">属性名称</param>
        /// <param name="defValue">默认值</param>
        /// <returns>返回节点属性值</returns>
        public string GetAttributeValue(string pNodePath_str, string pAttributeName_str, string defValue = "")
        {
            if (string.IsNullOrEmpty(pNodePath_str) || string.IsNullOrEmpty(pAttributeName_str))
                return defValue;
            return GetAttributeValue(SelectSingleNode(pNodePath_str), pAttributeName_str, defValue);
        }

        /// <summary>
        /// 获取节点属性
        /// </summary>
        /// <param name="xn">节点对象</param>
        /// <param name="pAttributeName_str">属性名称</param>
        /// <param name="defValue">默认值</param>
        /// <returns>返回节点属性值</returns>
        public string GetAttributeValue(XmlNode xn, string pAttributeName_str, string defValue = "")
        {
            if (string.IsNullOrEmpty(pAttributeName_str) || xn == null)
                return defValue;
            return xn.Attributes[pAttributeName_str].Value;
        }

        //public static string GetNodeValue(string nodeName,string defValue="")
        //{
        //    if (string.IsNullOrEmpty(nodeName))
        //        return defValue;
        //    //return nodeName.;

        //}
        #endregion


        #region 节点的修改功能
        /// <summary>
        /// 修改属性值
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <param name="attrbuteName">属性名</param>
        /// <param name="attrbuteValue">需赋得属性值</param>
        public void ModifyAttrValue(string nodePath, string attrbuteName, string attrbuteValue)
        {
            if (string.IsNullOrEmpty(nodePath) || string.IsNullOrEmpty(attrbuteName))
                return;

            XmlNode lTemp_xn = SelectSingleNode(nodePath);
            if (lTemp_xn == null) return;
            lTemp_xn.Attributes[attrbuteName].Value = attrbuteValue;
        }
        /// <summary>
        /// 修改节点值
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <param name="innerText">需付的节点值</param>
        /// <returns></returns>
        public void ModifyInnerText(string nodePath, string innerText)
        {
            if (string.IsNullOrEmpty(nodePath))
                return;

            XmlNode lTemp_xn = SelectSingleNode(nodePath);
            if (lTemp_xn == null) return;
            lTemp_xn.InnerText = innerText;
        }
        #endregion


        #region 节点的创建功能
        /// <summary>
        /// 创建新节点
        /// </summary>
        /// <param name="nodeParentPath">父路径</param>
        /// <param name="nodeName">节点名</param>
        /// <returns>返回新建节点</returns>
        public XmlNode CreateXmlNode(string nodeParentPath, string nodeName)
        {
            if (string.IsNullOrEmpty(nodeParentPath))
                return null;

            XmlNode xnParent = SelectSingleNode(nodeParentPath);
            if (xnParent == null) return null;

            return CreateXmlNode(xnParent, nodeName);
        }
        /// <summary>
        /// 创建新节点
        /// </summary>
        /// <param name="parentXn">父路径节点</param>
        /// <param name="nodeName">节点名</param>
        /// <returns>返回新建节点</returns>
        public XmlNode CreateXmlNode(XmlNode parentXn, string nodeName)
        {
            return CreateXmlNode(parentXn, nodeName, null);
        }
        /// <summary>
        /// 在指定的父节点下，创建新的子节点
        /// </summary>
        /// <param name="parentXn">父节点对象</param>
        /// <param name="nodeName">需创建的节点名</param>
        /// <param name="attrFindArgs">需创建的节点属性集合，可以为null</param>
        /// <returns>创建的节点对象,创建失败返回空</returns>
        public XmlNode CreateXmlNode(XmlNode parentXn, string nodeName, NameValueCollection attrFindArgs)
        {
            if (parentXn == null || string.IsNullOrEmpty(nodeName)) return null;

            //创建无属性节点
            XmlNode newNode = CreateNode(XmlNodeType.Element, nodeName, "");

            //给节点添加属性
            if (attrFindArgs != null)
            {
                for (int i = 0; i < attrFindArgs.Count; i++)
                {
                    XmlAttribute xmlAttr = CreateAttribute(attrFindArgs.Keys[i]);
                    xmlAttr.Value = attrFindArgs[i];
                    newNode.Attributes.Append(xmlAttr);
                }
            }

            //添加新节点到指定父节点
            parentXn.AppendChild(newNode);

            return newNode;
        }
        #endregion



        /// <summary>
        /// 默认保存为初始化使用的路径
        /// </summary>
        public void Save()
        {
            base.Save(fileName);
        }
    }
}
