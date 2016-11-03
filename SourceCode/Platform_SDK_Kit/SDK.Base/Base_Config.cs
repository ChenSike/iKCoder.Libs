using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace iKCoder_Platform_SDK_Kit
{
    public class Base_Config
    {
        XmlDocument _configDoc = new XmlDocument();
        string _fileName = "";
        bool isloadedDoc = false;
        Base_iInternalPlugin refInternalPlugin;


        public bool Is_LoadedDoc
        {
            get
            {
                return isloadedDoc;
            }
        }
    

        public Base_Config(string xmlData)
        {
            _configDoc = new XmlDocument();
            _configDoc.LoadXml(xmlData);
            isloadedDoc = true;            
        }

        public Base_Config(Base_iInternalPlugin refInternalPluginObj)
        {
            refInternalPlugin = refInternalPluginObj;            
        }
        public Base_Config()
        {
        }
        
        public Base_Config(string fileName,Base_iInternalPlugin refInternalPluginObj)
        {
            try
            {
                _fileName = fileName;
                _configDoc.Load(fileName);
                isloadedDoc = true;
                refInternalPlugin = refInternalPluginObj;       
            }
            catch(Base_AppExceptions err)
            {
                _configDoc.LoadXml("<root></root>");
                throw err;
            }
        }

        public XmlDocument Get_ConfigDocument()
        {
            return _configDoc;
        }

        public bool Create_NewConfigDocument()
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

        public bool Create_NewConfigDocument(string newFilePath)
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

        public bool doSave()
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

        public bool doOpen(string fileNamePath)
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

        public void doReset()
        {
            isloadedDoc = false;
        }

        public bool Is_DocumentExisted(string activeFolderPath, string fileName)
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

        public XmlNode Create_NewSession(string SessionName, string SessionValue,bool IsInternalPluginUsed,string configKey)
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
                XmlNode newSessionNode = XmlHelper.CreateNode(_configDoc, "session", result);
                XmlHelper.SetAttribute(newSessionNode, "name", SessionName);
                _configDoc.SelectSingleNode("/root").AppendChild(newSessionNode);
                return newSessionNode;
            }
        }

        public XmlNode Create_Item(XmlNode activeParentNode, string ItemName, string ItemValue, bool IsInternalPluginUsed, string configKey)
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
                XmlNode newItemNode = XmlHelper.CreateNode(_configDoc, "item", result);
                XmlHelper.SetAttribute(newItemNode, "name", ItemName);
                activeParentNode.AppendChild(newItemNode);
                return newItemNode;
            }
            catch
            {
                return null;
            }
        }

        public XmlNode Get_ItemNode(string SessionName, string ItemName)
        {
            XmlNode activeSession = Get_SessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNode itemNode = activeSession.SelectSingleNode("item[@name='" + ItemName + "']");
                return itemNode;
            }
            else
                return null;
        }

        public XmlNode Get_ItemNode(XmlNode parentNode, string ItemName)
        {
            if (parentNode != null)
            {                
                XmlNode itemNode = parentNode.SelectSingleNode("item[@name='" + ItemName + "']");
                return itemNode;            
            }
            else
                return null;
        }

        public XmlNodeList Get_ItemNodes(string SessionName, string ItemName)
        {
            XmlNode activeSession = Get_SessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNodeList itemNodes = activeSession.SelectNodes("item[@name='" + ItemName + "']");
                return itemNodes;
            }
            else
                return null;
        }

        public XmlNodeList Get_ItemNodes(string SessionName)
        {
            XmlNode activeSession = Get_SessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNodeList itemNodes = activeSession.SelectNodes("item");
                return itemNodes;
            }
            else
                return null;
        }

        public string Get_NodeValue(XmlNode activeNode, bool IsInternalPluginUsed,string configKey)
        {
            if (activeNode != null)
            {

                string sourceData =  XmlHelper.GetNodeValue("", activeNode);
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

        public XmlNode Get_SessionNode(string SessionName)
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

        public XmlNodeList Get_SessionNodes()
        {
            if (_configDoc != null)
            {
                return _configDoc.SelectNodes("/root/session");
            }
            else
                return null;
        }

        public List<XmlAttribute> Get_SessionAttrs(string SessionName)
        {
            List<XmlAttribute> resultList=new List<XmlAttribute>();
            if (Is_SessionExisted(SessionName))
            {
                XmlNode activeSessionNode = Get_SessionNode(SessionName);
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

        public bool Set_SessionAttr(string SessionName, string AttrName, string AttrValue, bool IsInternalPluginUsed,string configKey)
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
            XmlNode activeSessionNode = Get_SessionNode(SessionName);
            if (activeSessionNode != null)
            {
                XmlHelper.SetAttribute(activeSessionNode, AttrName, result);
                return true;
            }
            else
                return false;
        }

        public bool Set_SessionValue(string SessionName, string SessionValue, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "")
                return false;
            else
            {
                XmlNode activeSessionNode = Get_SessionNode(SessionName);
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

        public bool Set_ItemAttr(XmlNode Item, string AttrName, string AttrValue, bool IsInternalPluginUsed, string configKey)
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
                XmlHelper.SetAttribute(Item, AttrName, result);
                return true;
            }
        }

        public bool Set_InitDocument(string tagName, string tagValue, bool IsInternalPluginUsed, string configKey)
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
                    XmlHelper.SetAttribute(rootNode, tagName, IsInternalPluginUsed ? result : tagValue);
                    return true;
                }
            }
        }

        public bool Is_InitDocument(string tagName, string tagValue, bool IsInternalPluginUsed, string configKey)
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
                        return XmlHelper.GetNodeValue(tagName, rootNode) == tagValue ? true : false;
                    else
                    {
                        string result = "";
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", tagValue);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionSet(paramsDirc);
                        result = paramsDirc["return"].ToString();
                        return XmlHelper.GetNodeValue(tagName, rootNode) == result ? true : false;
                    }
                }
                    
            }
        }

        public bool Is_SessionExisted(string SessionName)
        {
            XmlNode activeSessionNode = Get_SessionNode(SessionName);
            if (activeSessionNode != null)
                return true;
            else
                return false;
        }

        public string Get_SessionValue(string SessionName, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "")
                return "";
            else
            {
                XmlNode activeSessionNode = Get_SessionNode(SessionName);
                string result = "";
                if (activeSessionNode != null)
                {
                    string value = XmlHelper.GetNodeValue("", activeSessionNode);
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

        public string Get_AttrValue(XmlNode ActiveNode, string AttrName, bool IsInternalPluginUsed, string configKey)
        {
            if (ActiveNode == null)
                return "";
            else
            {
                string attrResult = XmlHelper.GetAttrValue(ActiveNode, AttrName);
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

        public string Get_ItemValue(string SessionName, string ItemName, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "" || ItemName=="")
                return "";
            else
            {
                XmlNode activeItemNode = Get_ItemNode(SessionName, ItemName);
                string attrResult = XmlHelper.GetNodeValue("", activeItemNode);
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

        public string Get_ItemValue(XmlNode parentNode, bool IsInternalPluginUsed, string configKey)
        {
            if (parentNode == null)
                return "";
            else
            {
                string attrResult = XmlHelper.GetNodeValue("", parentNode);
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

        

        public bool Remove_Session(string SessionName)
        {
            if (Is_SessionExisted(SessionName))
            {
                XmlNode activeSessionNode = Get_SessionNode(SessionName);
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

        public bool Remove_SessionItem(string SessionName, string SessionItemName)
        {
            if (SessionName != "" && SessionItemName != "")
            {
                XmlNode activeSessionNode = Get_SessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    XmlNode itemNode = Get_ItemNode(SessionName, SessionItemName);
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

        public bool Remove_ActiveItem(XmlNode ParentNode, string ActiveItemName)
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

        public bool Remove_SessionAttr(string SessionName, string SessionAttrName)
        {
            if (Is_SessionExisted(SessionName))
            {
                XmlNode activeSessionNode = Get_SessionNode(SessionName);
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
