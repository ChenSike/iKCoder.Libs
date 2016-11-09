using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Base_AppRuuningLogServices
    {
        private XmlDocument _AppRunningDoc = new XmlDocument();
        private string _LogStruct = "<root></root>";
        public string LogFilePath = "";
        private FileStream fs;
        private StreamWriter sw;

        public class_Base_AppRuuningLogServices(string ActiveLogFilePath)
        {
            LoadLog(ActiveLogFilePath);
        }

        public class_Base_AppRuuningLogServices()
        {
            _AppRunningDoc.LoadXml(_LogStruct);
        }

        public XmlDocument GetLogDoc()
        {
            return _AppRunningDoc;
        }

        public void AddNewRecord(string key, DateTime activeTime, bool isSystemInfo, bool isFaild, string Key, string Message)
        {
            XmlNode parent = this._AppRunningDoc.SelectSingleNode("/root/item[@key='" + key + "']");
            if (parent == null)
            {
                parent = class_XmlHelper.CreateNode(this._AppRunningDoc, "item", "");
                class_XmlHelper.SetAttribute(parent, "key", key);
                this._AppRunningDoc.SelectSingleNode("/root").AppendChild(parent);
            }
            XmlNode node2 = parent.SelectSingleNode("point[@time='" + activeTime.ToString() + "']");
            if (node2 == null)
            {
                node2 = class_XmlHelper.CreateNode(this._AppRunningDoc, "point", "");
                class_XmlHelper.SetAttribute(node2, "time", DateTime.Now.ToString());
                parent.AppendChild(node2);
            }
            XmlNode newChild = node2.SelectSingleNode(isFaild ? "fail" : "pass");
            if (newChild == null)
            {
                newChild = class_XmlHelper.CreateNode(this._AppRunningDoc, isFaild ? "fail" : "pass", "");
                node2.AppendChild(newChild);
            }
            if (newChild.SelectSingleNode(isSystemInfo ? "systeminfo" : "records") == null)
            {
                XmlNode node4 = class_XmlHelper.CreateNode(this._AppRunningDoc, isSystemInfo ? "systeminfo" : "records", "");
                newChild.AppendChild(node4);
            }
            XmlNode node5 = class_XmlHelper.CreateNode(this._AppRunningDoc, "item", "");
            class_XmlHelper.SetAttribute(node5, "key", Key);
            class_XmlHelper.SetAttribute(node5, "message", Message);
            newChild.AppendChild(node5);
            if (this.LogFilePath != "")
                this.ActionSaveLog(); 
        }

        public void AddLogItem(string header, Dictionary<string, string> attrList)
        {
            if (attrList.Count != 0)
            {
                XmlNode newChild = class_XmlHelper.CreateNode(this._AppRunningDoc, "item", "");
                this._AppRunningDoc.SelectSingleNode("/root").AppendChild(newChild);
                class_XmlHelper.SetAttrValue(newChild, "header", header);
                foreach (string str in attrList.Keys)
                {
                    class_XmlHelper.SetAttribute(newChild, str, attrList[str]);
                }
                if (this.LogFilePath != "")
                    this.ActionSaveLog();             

            }
        }

        public void AddLogItem(string header, string content)
        {
            XmlNode newChild = class_XmlHelper.CreateNode(this._AppRunningDoc, "item", "");
            this._AppRunningDoc.SelectSingleNode("/root").AppendChild(newChild);
            class_XmlHelper.SetAttribute(newChild, "header", header);
            class_XmlHelper.SetNodeValue(newChild, content);
            if (this.LogFilePath != "")
                this.ActionSaveLog();   
        }

        public void ActionSaveLog()
        {
            lock (this._AppRunningDoc)
            {
                this._AppRunningDoc.Save(this.LogFilePath);
            }
        }

        public void ActionSaveLog(string fileName)
        {
            lock (this._AppRunningDoc)
            {
                this._AppRunningDoc.Save(fileName);
            }
        }        

        public string GetLogItem(string header)
        {
            XmlNode node = this._AppRunningDoc.SelectSingleNode("/root/item[@header='" + header + "']");
            if (node != null)
            {
                return node.InnerText;
            }
            return "";
        }

        public string GetLogItem(string header, string attrName)
        {
            XmlNode node = this._AppRunningDoc.SelectSingleNode("/root/item[@header='" + header + "']");
            if (node != null)
            {
                return class_XmlHelper.GetNodeValue("@" + attrName, node);
            }
            return "";
        }

        public void LoadLog(string logFileName)
        {
            FileInfo fi = new FileInfo(logFileName);
            this.LogFilePath = logFileName;            
            if (!fi.Exists)
            {
                this.fs = new FileStream(this.LogFilePath, FileMode.Create);
                this.sw = new StreamWriter(this.fs);
                this.sw.WriteLine(this._LogStruct);
                this.sw.Flush();
                this.fs.Flush();
                this.sw.Close();
                this.fs.Close();
            }
            this._AppRunningDoc = new XmlDocument();
            this._AppRunningDoc.Load(this.LogFilePath);
        }

    }
}

