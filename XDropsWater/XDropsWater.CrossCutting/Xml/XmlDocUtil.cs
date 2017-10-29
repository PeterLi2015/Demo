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
        /// ����xml���ݹ����࣬�̳���XmlDocument
        /// </summary>
        /// <param name="fileName">xml�ļ�����</param>
        public XmlDocUtil(string fileName)
        {
            Load(fileName);
        }

        /// <summary>
        /// ���ػ���ķ���
        /// </summary>
        /// <param name="fileName"></param>
        public new void Load(string fileName)
        {
            if (!File.Exists(fileName))
                throw new Exception(string.Format("����fileName�ļ�·��[{0}]�����ڣ�", fileName));

            this.fileName = fileName;
            base.Load(fileName);
        }

        /// <summary>
        /// xml�ļ�·��
        /// </summary>
        private string fileName = string.Empty;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        #region �ڵ��ȡ����
        /// <summary>
        /// ���ָ�������ڵ���ӽڵ㼯��
        /// </summary>
        /// <param name="NodeName">�ڵ�·��</param>
        /// <returns></returns>
        public XmlNodeList GetChildNodes(string NodeName)
        {
            if (string.IsNullOrEmpty(NodeName)) return null;

            XmlNode lTemp_xn = SelectSingleNode(NodeName);
            if (lTemp_xn == null) return null;

            return lTemp_xn.ChildNodes;
        }
        /// <summary>
        /// ��ȡ�ڵ�����
        /// </summary>
        /// <param name="pNodePath_str">Xpath���ʽ</param>
        /// <param name="pAttributeName_str">��������</param>
        /// <param name="defValue">Ĭ��ֵ</param>
        /// <returns>���ؽڵ�����ֵ</returns>
        public string GetAttributeValue(string pNodePath_str, string pAttributeName_str, string defValue = "")
        {
            if (string.IsNullOrEmpty(pNodePath_str) || string.IsNullOrEmpty(pAttributeName_str))
                return defValue;
            return GetAttributeValue(SelectSingleNode(pNodePath_str), pAttributeName_str, defValue);
        }

        /// <summary>
        /// ��ȡ�ڵ�����
        /// </summary>
        /// <param name="xn">�ڵ����</param>
        /// <param name="pAttributeName_str">��������</param>
        /// <param name="defValue">Ĭ��ֵ</param>
        /// <returns>���ؽڵ�����ֵ</returns>
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


        #region �ڵ���޸Ĺ���
        /// <summary>
        /// �޸�����ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <param name="attrbuteName">������</param>
        /// <param name="attrbuteValue">�踳������ֵ</param>
        public void ModifyAttrValue(string nodePath, string attrbuteName, string attrbuteValue)
        {
            if (string.IsNullOrEmpty(nodePath) || string.IsNullOrEmpty(attrbuteName))
                return;

            XmlNode lTemp_xn = SelectSingleNode(nodePath);
            if (lTemp_xn == null) return;
            lTemp_xn.Attributes[attrbuteName].Value = attrbuteValue;
        }
        /// <summary>
        /// �޸Ľڵ�ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <param name="innerText">�踶�Ľڵ�ֵ</param>
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


        #region �ڵ�Ĵ�������
        /// <summary>
        /// �����½ڵ�
        /// </summary>
        /// <param name="nodeParentPath">��·��</param>
        /// <param name="nodeName">�ڵ���</param>
        /// <returns>�����½��ڵ�</returns>
        public XmlNode CreateXmlNode(string nodeParentPath, string nodeName)
        {
            if (string.IsNullOrEmpty(nodeParentPath))
                return null;

            XmlNode xnParent = SelectSingleNode(nodeParentPath);
            if (xnParent == null) return null;

            return CreateXmlNode(xnParent, nodeName);
        }
        /// <summary>
        /// �����½ڵ�
        /// </summary>
        /// <param name="parentXn">��·���ڵ�</param>
        /// <param name="nodeName">�ڵ���</param>
        /// <returns>�����½��ڵ�</returns>
        public XmlNode CreateXmlNode(XmlNode parentXn, string nodeName)
        {
            return CreateXmlNode(parentXn, nodeName, null);
        }
        /// <summary>
        /// ��ָ���ĸ��ڵ��£������µ��ӽڵ�
        /// </summary>
        /// <param name="parentXn">���ڵ����</param>
        /// <param name="nodeName">�贴���Ľڵ���</param>
        /// <param name="attrFindArgs">�贴���Ľڵ����Լ��ϣ�����Ϊnull</param>
        /// <returns>�����Ľڵ����,����ʧ�ܷ��ؿ�</returns>
        public XmlNode CreateXmlNode(XmlNode parentXn, string nodeName, NameValueCollection attrFindArgs)
        {
            if (parentXn == null || string.IsNullOrEmpty(nodeName)) return null;

            //���������Խڵ�
            XmlNode newNode = CreateNode(XmlNodeType.Element, nodeName, "");

            //���ڵ��������
            if (attrFindArgs != null)
            {
                for (int i = 0; i < attrFindArgs.Count; i++)
                {
                    XmlAttribute xmlAttr = CreateAttribute(attrFindArgs.Keys[i]);
                    xmlAttr.Value = attrFindArgs[i];
                    newNode.Attributes.Append(xmlAttr);
                }
            }

            //����½ڵ㵽ָ�����ڵ�
            parentXn.AppendChild(newNode);

            return newNode;
        }
        #endregion



        /// <summary>
        /// Ĭ�ϱ���Ϊ��ʼ��ʹ�õ�·��
        /// </summary>
        public void Save()
        {
            base.Save(fileName);
        }
    }
}
