using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Base_Config
    {
        XmlDocument _configDoc = new XmlDocument();
        string _fileName = "";
        bool isloadedDoc = false;
        interface_Base_iInternalPlugin refInternalPlugin;


        public bool Is_LoadedDoc
        {
            get
            {
                return isloadedDoc;
            }
        }
    

        public class_Base_Config(string xmlData)
        {
            _configDoc = new XmlDocument();
            _configDoc.LoadXml(xmlData);
            isloadedDoc = true;            
        }

        public class_Base_Config(interface_Base_iInternalPlugin refInternalPluginObj)
        {
            refInternalPlugin = refInternalPluginObj;            
        }
        public class_Base_Config()
        {
        }

        public class_Base_Config(string fileName, interface_Base_iInternalPlugin refInternalPluginObj)
        {
            try
            {
                _fileName = fileName;
                _configDoc.Load(fileName);
                isloadedDoc = true;
                refInternalPlugin = refInternalPluginObj;       
            }
            catch(class_Base_AppExceptions err)
            {
                _configDoc.LoadXml("<root></root>");
                throw err;
            }
        }

        public XmlDocument GetConfigDocument()
        {
            return _configDoc;
        }

        public bool CreateNewConfigDocument()
        {
            try
            {
                _configDoc.LoadXml("<root></root>");
                _configDoc.Save(_fileName);
                isloadedDoc = true;
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool CreateNewConfigDocument(string newFilePath)
        {
            try
            {
                _configDoc.LoadXml("<root></root>");
                _configDoc.Save(newFilePath);                
                _fileName = newFilePath;
                isloadedDoc = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DoSave()
        {
            try
            {
                lock (_configDoc)
                {
                    _configDoc.Save(_fileName);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DoOpen(string fileNamePath)
        {
            try
            {
                _configDoc = new XmlDocument();
                _configDoc.Load(fileNamePath);
                _fileName = fileNamePath;
                isloadedDoc = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void DoReset()
        {
            isloadedDoc = false;
        }

        public bool IsDocumentExisted(string activeFolderPath, string fileName)
        {
            if (fileName == "")
                return false;
            else
            {
                DirectoryInfo activeDIObj = new DirectoryInfo(activeFolderPath == "" ? Environment.CurrentDirectory : activeFolderPath);
                FileInfo[] activeFiles = activeDIObj.GetFiles("*.xml");
                foreach (FileInfo fi in activeFiles)
                {
                    if (fi.FullName.Contains(fileName))
                        return true;
                }
                return false;
            }
        }

        public XmlNode CreateNewSession(string SessionName, string SessionValue,bool IsInternalPluginUsed,string configKey)
        {
            if (SessionName == "")
                return null;
            else
            {
                string result = SessionValue;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", SessionValue);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionSet(paramsDirc);
                    result = paramsDirc["return"].ToString();
                }
                XmlNode newSessionNode = class_XmlHelper.CreateNode(_configDoc, "session", result);
                class_XmlHelper.SetAttribute(newSessionNode, "name", SessionName);
                _configDoc.SelectSingleNode("/root").AppendChild(newSessionNode);
                return newSessionNode;
            }
        }

        public XmlNode CreateItem(XmlNode activeParentNode, string ItemName, string ItemValue, bool IsInternalPluginUsed, string configKey)
        {
            if (activeParentNode == null)
                return null;
            string result = ItemValue;
            if (IsInternalPluginUsed && refInternalPlugin != null)
            {
                Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                paramsDirc.Add("key", configKey);
                paramsDirc.Add("value", ItemValue);
                paramsDirc.Add("return", "");
                refInternalPlugin.actionSet(paramsDirc);
                result = paramsDirc["return"].ToString();
            }
            try
            {
                XmlNode newItemNode = class_XmlHelper.CreateNode(_configDoc, "item", result);
                class_XmlHelper.SetAttribute(newItemNode, "name", ItemName);
                activeParentNode.AppendChild(newItemNode);
                return newItemNode;
            }
            catch
            {
                return null;
            }
        }

        public XmlNode GetItemNode(string SessionName, string ItemName)
        {
            XmlNode activeSession = GetSessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNode itemNode = activeSession.SelectSingleNode("item[@name='" + ItemName + "']");
                return itemNode;
            }
            else
                return null;
        }

        public XmlNode GetItemNode(XmlNode parentNode, string ItemName)
        {
            if (parentNode != null)
            {                
                XmlNode itemNode = parentNode.SelectSingleNode("item[@name='" + ItemName + "']");
                return itemNode;            
            }
            else
                return null;
        }

        public XmlNodeList GetItemNodes(string SessionName, string ItemName)
        {
            XmlNode activeSession = GetSessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNodeList itemNodes = activeSession.SelectNodes("item[@name='" + ItemName + "']");
                return itemNodes;
            }
            else
                return null;
        }

        public XmlNodeList GetItemNodes(string SessionName)
        {
            XmlNode activeSession = GetSessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNodeList itemNodes = activeSession.SelectNodes("item");
                return itemNodes;
            }
            else
                return null;
        }

        public string GetNodeValue(XmlNode activeNode, bool IsInternalPluginUsed,string configKey)
        {
            if (activeNode != null)
            {

                string sourceData =  class_XmlHelper.GetNodeValue("", activeNode);
                string resultData="";
                if (IsInternalPluginUsed && refInternalPlugin!=null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", sourceData);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    resultData = paramsDirc["return"].ToString();                    
                }
                else
                {
                    resultData = sourceData;
                }
                return resultData;
            }
            else
                return "";
        }        

        public XmlNode GetSessionNode(string SessionName)
        {
            if (SessionName == "")
                return null;
            else
            {
                XmlNode activeSessionNode = _configDoc.SelectSingleNode("/root/session[@name='" + SessionName + "']");
                if (activeSessionNode != null)
                    return activeSessionNode;
                else
                    return null;
            }
        }

        public XmlNodeList GetSessionNodes()
        {
            if (_configDoc != null)
            {
                return _configDoc.SelectNodes("/root/session");
            }
            else
                return null;
        }

        public List<XmlAttribute> GetSessionAttrs(string SessionName)
        {
            List<XmlAttribute> resultList=new List<XmlAttribute>();
            if (IsSessionExisted(SessionName))
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    foreach (XmlAttribute activeAttr in activeSessionNode.Attributes)
                        resultList.Add(activeAttr);
                }
                return resultList;
            }
            else
                return resultList;
        }

        public bool SetSessionAttr(string SessionName, string AttrName, string AttrValue, bool IsInternalPluginUsed,string configKey)
        {
            if(SessionName=="" || AttrName=="")
                return false;
            string activeAttrValue = AttrValue;
            string result = AttrValue;
            if (IsInternalPluginUsed && refInternalPlugin != null)
            {
                Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                paramsDirc.Add("key", configKey);
                paramsDirc.Add("value", AttrValue);
                paramsDirc.Add("return", "");
                refInternalPlugin.actionGet(paramsDirc);
                result = paramsDirc["return"].ToString();                             
            }
            XmlNode activeSessionNode = GetSessionNode(SessionName);
            if (activeSessionNode != null)
            {
                class_XmlHelper.SetAttribute(activeSessionNode, AttrName, result);
                return true;
            }
            else
                return false;
        }

        public bool SetSessionValue(string SessionName, string SessionValue, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "")
                return false;
            else
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    string SessionValueResult = "";
                    if (IsInternalPluginUsed && refInternalPlugin != null)
                    {
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", SessionValue);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionSet(paramsDirc);
                        SessionValueResult = paramsDirc["return"].ToString();      
                    }
                    else
                        SessionValueResult = SessionValue;
                    activeSessionNode.InnerText = SessionValueResult;
                    return true;
                }
                else
                    return false;

            }
        }

        public bool SetItemAttr(XmlNode Item, string AttrName, string AttrValue, bool IsInternalPluginUsed, string configKey)
        {
            if (Item == null)
                return false;
            else
            {
                string result = AttrValue;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", AttrValue);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionSet(paramsDirc);
                    result = paramsDirc["return"].ToString();

                }
                class_XmlHelper.SetAttribute(Item, AttrName, result);
                return true;
            }
        }

        public bool SetInitDocument(string tagName, string tagValue, bool IsInternalPluginUsed, string configKey)
        {
            if (_configDoc == null)
                return false;
            else
            {
                XmlNode rootNode = _configDoc.SelectSingleNode("/root");
                if (rootNode == null)
                    return false;
                else
                {
                    string result = "";
                    if (IsInternalPluginUsed && refInternalPlugin != null)
                    {
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", tagValue);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionSet(paramsDirc);
                        result = paramsDirc["return"].ToString();
                    }
                    class_XmlHelper.SetAttribute(rootNode, tagName, IsInternalPluginUsed ? result : tagValue);
                    return true;
                }
            }
        }

        public bool IsInitDocument(string tagName, string tagValue, bool IsInternalPluginUsed, string configKey)
        {
            if (_configDoc == null)
                return false;
            else
            {
                XmlNode rootNode = _configDoc.SelectSingleNode("/root");
                if (rootNode == null)
                    return false;
                else
                {
                    if (!IsInternalPluginUsed || refInternalPlugin==null)
                        return class_XmlHelper.GetNodeValue(tagName, rootNode) == tagValue ? true : false;
                    else
                    {
                        string result = "";
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", tagValue);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionSet(paramsDirc);
                        result = paramsDirc["return"].ToString();
                        return class_XmlHelper.GetNodeValue(tagName, rootNode) == result ? true : false;
                    }
                }
                    
            }
        }

        public bool IsSessionExisted(string SessionName)
        {
            XmlNode activeSessionNode = GetSessionNode(SessionName);
            if (activeSessionNode != null)
                return true;
            else
                return false;
        }

        public string GetSessionValue(string SessionName, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "")
                return "";
            else
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                string result = "";
                if (activeSessionNode != null)
                {
                    string value = class_XmlHelper.GetNodeValue("", activeSessionNode);
                    result = value;
                    if (IsInternalPluginUsed && refInternalPlugin != null)
                    {
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", value);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionGet(paramsDirc);
                        result = paramsDirc["return"].ToString();
                    }
                }
                
                return result;                
            }
        }

        public string GetAttrValue(XmlNode ActiveNode, string AttrName, bool IsInternalPluginUsed, string configKey)
        {
            if (ActiveNode == null)
                return "";
            else
            {
                string attrResult = class_XmlHelper.GetAttrValue(ActiveNode, AttrName);
                string result = attrResult;
                if (IsInternalPluginUsed && refInternalPlugin!=null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", attrResult);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    result = paramsDirc["return"].ToString();                    
                }
                return result;
            }
        }

        public string GetItemValue(string SessionName, string ItemName, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "" || ItemName=="")
                return "";
            else
            {
                XmlNode activeItemNode = GetItemNode(SessionName, ItemName);
                string attrResult = class_XmlHelper.GetNodeValue("", activeItemNode);
                string Result = attrResult;
                if (IsInternalPluginUsed && refInternalPlugin!=null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", attrResult);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    Result = paramsDirc["return"].ToString();     
                }
                return Result;
            }
        }

        public string GetItemValue(XmlNode parentNode, bool IsInternalPluginUsed, string configKey)
        {
            if (parentNode == null)
                return "";
            else
            {
                string attrResult = class_XmlHelper.GetNodeValue("", parentNode);
                string Result = attrResult;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", attrResult);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    Result = paramsDirc["return"].ToString();                        
                }
                return Result;
            }
        }

        

        public bool RemoveSession(string SessionName)
        {
            if (IsSessionExisted(SessionName))
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    activeSessionNode.ParentNode.RemoveChild(activeSessionNode);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool RemoveSessionItem(string SessionName, string SessionItemName)
        {
            if (SessionName != "" && SessionItemName != "")
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    XmlNode itemNode = GetItemNode(SessionName, SessionItemName);
                    if (itemNode != null)
                    {
                        activeSessionNode.RemoveChild(itemNode);
                        return true;
                    }
                    else
                        return false;

                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool RemoveActiveItem(XmlNode ParentNode, string ActiveItemName)
        {
            if (ParentNode == null)
                return false;
            else
            {
                XmlNode activeItemNode = ParentNode.SelectSingleNode("item[@name='" + ActiveItemName + "']");
                if (activeItemNode != null)
                {
                    ParentNode.RemoveChild(activeItemNode);
                    return true;
                }
                else
                    return false;
            }
        }

        public bool RemoveSessionAttr(string SessionName, string SessionAttrName)
        {
            if (IsSessionExisted(SessionName))
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                XmlAttribute activeAttr = activeSessionNode.Attributes[SessionAttrName];
                if (activeAttr != null)
                {
                    activeSessionNode.Attributes.Remove(activeAttr);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

    }
}
