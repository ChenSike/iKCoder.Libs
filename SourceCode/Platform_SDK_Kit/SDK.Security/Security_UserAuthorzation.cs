using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace iKCoder_Platform_SDK_Kit
{

    public class Security_Authorzation
    {

        private XmlDocument activeUserDoc = new XmlDocument();       

        public Security_Authorzation()
        {
        }      

        public bool InitUserDoc(XmlDocument UserDoc)
        {            
            activeUserDoc=UserDoc;
            return true;
        }

        public bool Action_isUserExisted(string username,string userDesKey)
        {
            string resultUsername = "";
            Security_DES activeDesObj = new Security_DES(userDesKey);
            activeDesObj.DESCoding(username, out resultUsername);
            return (this.activeUserDoc.SelectSingleNode("/root/item[@username='" + resultUsername + "']") != null);
        }

        
        public bool Action_CreateNewUser(string username, string pwd, Dictionary<string, string> attrs,string userDesKey)
        {
            try
            {
                if (!this.Action_isUserExisted(username,userDesKey))
                {
                    Security_DES activeDesObj = new Security_DES(userDesKey);
                    XmlNode node = this.activeUserDoc.SelectSingleNode("/root");
                    XmlNode newitemNode = class_XmlHelper.CreateNode(this.activeUserDoc, "item", "");
                    XmlNode groupListNode = class_XmlHelper.CreateNode(this.activeUserDoc,"grouplist","");
                    newitemNode.AppendChild(groupListNode);
                    string resultUsername = "";
                    activeDesObj.DESCoding(username, out resultUsername);
                    class_XmlHelper.SetAttribute(newitemNode, "username", resultUsername);
                    string resultPwd = "";
                    activeDesObj.DESCoding(pwd, out resultPwd);
                    class_XmlHelper.SetAttribute(newitemNode, "pwd", resultPwd);
                    foreach (string key in attrs.Keys)
                    {
                        string resultType = "";
                        activeDesObj.DESCoding(attrs[key], out resultType);
                        class_XmlHelper.SetAttribute(newitemNode, key, resultType);
                    }
                    node.AppendChild(newitemNode);
                    Security_UserItem obj = new Security_UserItem(userDesKey);
                    obj.UserName = username;
                    obj.UserPassword = pwd;
                    Pools_Security.AddItemToUserlist(obj);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Action_ChgUserAttra(string activeUserName, string attrName, string attrValue, string userDesKey)
        {
            if (Action_isUserExisted(activeUserName, userDesKey))
            {
                XmlNode activeUserNode = Action_GetActiveUserNode(activeUserName, userDesKey);
                string objAttrValue = "";
                Security_DES activeDes = new Security_DES(userDesKey);
                activeDes.DESCoding(attrValue, out objAttrValue);
                class_XmlHelper.SetAttribute(activeUserNode, attrName, objAttrValue);
                return true;
            }
            else
                return false;
        }


        public bool Action_IsGroupExisted(string groupName,string desGroupKey)
        {
            if(groupName=="" || desGroupKey=="")
            {
                return false;
            }
            else
            {
                if(this.activeUserDoc!=null)
                {
                    Security_DES activeDesObj = new Security_DES(desGroupKey);
                    string desGroupName="";
                    activeDesObj.DESCoding(groupName,out desGroupName);                        
                    XmlNode group=this.activeUserDoc.SelectSingleNode("/root/group[@name='"+desGroupName+"']");
                    if(group!=null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        public Security_UserGroup Action_CreateGroup(string groupName,string desGroupKey)
        {
            if(groupName=="" || desGroupKey=="")
                return null;
            else
            {
                if(!Action_IsGroupExisted(groupName,desGroupKey))
                {
                    Security_DES activeDesObj = new Security_DES(desGroupKey);
                    XmlNode newGroup=class_XmlHelper.CreateNode(activeUserDoc, "group","");
                    string activeGroupName="";
                    activeDesObj.DESCoding(groupName,out activeGroupName);
                    class_XmlHelper.SetAttribute(newGroup,"name",activeGroupName);
                    activeUserDoc.SelectSingleNode("/root").AppendChild(newGroup);
                    Security_UserGroup newGroupObj=new Security_UserGroup(desGroupKey);
                    newGroupObj.GroupKey=desGroupKey;
                    newGroupObj.GroupKeyName=groupName;
                    Pools_Security.AddItemToGrouplist(newGroupObj);
                    return newGroupObj;
                }
                else
                    return null;
            }
        }

        public string Action_GetUserAttr(string conditionKey, string conditionValue, string attrName, bool isEncry, string userDesKey)
        {
            if (conditionKey == "" || attrName == "")
                return "";
            else
            {
                Security_DES activeDesObj = new Security_DES(userDesKey);
                string keyvalue = "";
                if (isEncry)
                    activeDesObj.DESCoding(conditionValue, out keyvalue);
                else
                    keyvalue = conditionValue;
                XmlNode activeNode = this.activeUserDoc.SelectSingleNode("/root/item[@" + conditionKey + "='" + keyvalue + "']");
                if (activeNode != null)
                {
                    string value = "";
                    if (isEncry)
                    {
                        if (class_XmlHelper.GetNodeValue(attrName, activeNode) != "")
                            activeDesObj.DESDecoding(class_XmlHelper.GetNodeValue(attrName.StartsWith("@") ? attrName : "@" + attrName, activeNode), out value);

                    }
                    else
                        value = class_XmlHelper.GetNodeValue(attrName, activeNode);
                    return value;
                }
                else
                    return "";
            }
        }

        public bool Action_InitUsersToPool(string userDesKey)
        {
            if (this.activeUserDoc != null)
            {
                XmlNodeList allUserNodes = this.activeUserDoc.SelectNodes("/root/item");
                Security_DES activeDesObj = new Security_DES(userDesKey);
                foreach (XmlNode activeUserNode in allUserNodes)
                {
                    Security_UserItem obj = new Security_UserItem(userDesKey);
                    string result="";
                    activeDesObj.DESDecoding(class_XmlHelper.GetNodeValue("@username", activeUserNode), out result);
                    obj.UserName = result;
                    activeDesObj.DESDecoding(class_XmlHelper.GetNodeValue("@password", activeUserNode), out result);
                    obj.UserPassword = result;
                    obj.UserKey=userDesKey;
                    Pools_Security.AddItemToUserlist(obj);
                }
                return true;
            }
            else
                return false;
        }

        public XmlNode Action_GetActiveUserNode(string activeUserName, string userDesKey)
        {
            if (activeUserName == "" || userDesKey == "")
                return null;
            else
            {
                Security_DES activeDesObj = new Security_DES(userDesKey);
                string userName = "";
                activeDesObj.DESCoding(activeUserName, out userName);
                XmlNode activeUserNode;
                activeUserNode = activeUserDoc.SelectSingleNode("/root/item[@username='" + activeUserName + "']");
                if (activeUserNode != null)
                    return activeUserNode;
                else
                    return null;
            }
        }

        public bool Action_SetGroupToUser(string activeUserName, string userDesKey, string activeGroupName, string groupDesKey)
        {
            if (activeUserName == "" || userDesKey == "" || activeGroupName == "" || groupDesKey == "")
                return false;
            else
            {
                if (!Action_isUserExisted(activeUserName, userDesKey))
                    return false;
                if (!Action_IsGroupExisted(activeGroupName, groupDesKey))
                    return false;
                XmlNode activeUserNode = Action_GetActiveUserNode(activeUserName, userDesKey);
                XmlNode groupListNode = activeUserNode.SelectSingleNode("grouplist");
                if (groupListNode.SelectSingleNode("item[@value='" + activeGroupName + "']") == null)
                {
                    XmlNode newAssignedGroup = class_XmlHelper.CreateNode(activeUserDoc, "item", "");
                    class_XmlHelper.SetAttribute(newAssignedGroup, "value", groupDesKey);
                    groupListNode.AppendChild(newAssignedGroup);
                    Security_UserItem activeUserItem = Pools_Security.GetItemFromUserlist(activeUserName);
                    activeUserItem.Action_InsertGroup(activeGroupName);                    
                }
                return true;
            }
        }

        public bool Action_DelGroupFromUser(string activeUserName, string userDesKey, string activeGroupName, string groupDesKey)
        {
            if (activeUserName == "" || userDesKey == "" || activeGroupName == "" || groupDesKey == "")
                return false;
            else
            {
                if (!Action_isUserExisted(activeUserName, userDesKey))
                    return false;
                if (!Action_IsGroupExisted(activeGroupName, groupDesKey))
                    return false;
                XmlNode activeUserNode = Action_GetActiveUserNode(activeUserName, userDesKey);
                XmlNode groupListNode = activeUserNode.SelectSingleNode("grouplist");
                XmlNode assignedGroupNode = groupListNode.SelectSingleNode("item[@value='" + activeGroupName + "']");
                if (assignedGroupNode != null)
                {
                    groupListNode.RemoveChild(assignedGroupNode);
                    Security_UserItem activeUserItem=Pools_Security.GetItemFromUserlist(activeUserName);
                    activeUserItem.Action_DelGroup(activeGroupName);
                    return true;
                }
                else
                    return false;
                
            }
        }

        public bool Action_RemoveUser(string activeUserName, string userDesKey)
        {
            if (activeUserName == "" || userDesKey == "")
                return false;
            else
            {
                if (!Action_isUserExisted(activeUserName, userDesKey))
                    return false;
                else
                {
                    XmlNode activeUserNode = Action_GetActiveUserNode(activeUserName, userDesKey);
                    if (activeUserNode != null)
                    {
                        activeUserDoc.SelectSingleNode("/root").RemoveChild(activeUserNode);
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        public bool Action_RemoveGroup(string activeGroupName, string groupDesKey)
        {
            if (activeGroupName == "" || groupDesKey == "")
                return false;
            else
            {
                if (!Action_IsGroupExisted(activeGroupName, groupDesKey))
                    return false;
                else
                {
                    foreach (string activeUserName in Pools_Security.ActiveUsers.Keys)
                    {
                        Security_UserItem activeUser = Pools_Security.GetItemFromUserlist(activeUserName);
                        activeUser.Action_DelGroup(activeGroupName);
                        Action_DelGroupFromUser(activeUserName, activeUser.UserKey, activeGroupName, groupDesKey);
                    }                  
                    string activeDESGroupName = "";
                    Security_DES activeDesObj = new Security_DES(groupDesKey);
                    activeDesObj.DESCoding(activeGroupName, out activeDESGroupName);
                    XmlNode activeGroupNode = activeUserDoc.SelectSingleNode("/root/group[@name='" + activeDESGroupName + "']");
                    if (activeGroupNode != null)
                    {
                        activeUserDoc.SelectSingleNode("/root").RemoveChild(activeGroupNode);
                        return true;
                    }
                    else
                        return false;
                }
            }
        }        
    }

    public class Security_UserAuthorzationDoc
    {
        private const string Security_Flag = "NOx_SDK.SecurityDoc";
        private Security_DES _des = new Security_DES();
        private class_Data_SqlDataHelper _sqlDataHelper = new class_Data_SqlDataHelper();
        public string activeSecurityFile = "";

        public Security_UserAuthorzationDoc(Security_DES activeDES)
        {           
            _des = activeDES;
        }

        public Security_UserAuthorzationDoc()
        {
        }

        public bool Action_IsSecurityDoc(XmlDocument doc)
        {
            if (doc != null)
            {
                XmlNode rootNode = doc.SelectSingleNode("/root");
                if (rootNode != null)
                {
                    string result = "";
                    string type=class_XmlHelper.GetNodeValue("@doctype",rootNode);
                    _des.DESDecoding(type, out result);
                    if (result == Security_Flag)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool Action_SigDoc(XmlDocument doc)
        {
            if (!Action_IsSecurityDoc(doc))
            {
                XmlNode rootNode = doc.SelectSingleNode("/root");
                if (rootNode != null)
                {
                    string result = "";
                    _des.DESCoding(Security_Flag, out result);
                    class_XmlHelper.SetAttribute(rootNode, "doctype", result);
                    return true;                        
                }
                else
                    return false;
            }
            else
                return false;
        }


        public XmlDocument Action_CreateDocForFile(string fileName)
        {
            try
            {
                XmlDocument _doc=new XmlDocument();                               
                _doc.LoadXml("<root></root>");
                Action_SigDoc(_doc);
                _doc.Save(fileName);                
                return _doc;
            }
            catch
            {
                return null;
            }
        }

        public XmlDocument Action_CreateDoc()
        {
            try
            {
                XmlDocument _doc = new XmlDocument();
                _doc.LoadXml("<root></root>");
                Action_SigDoc(_doc);
                return _doc;
            }
            catch
            {
                return null;
            }
        }

        public XmlDocument Action_LoadDocFromFile(string filename)
        {
            try
            {
                XmlDocument _doc = new XmlDocument();
                _doc.Load(filename);
                if (Action_IsSecurityDoc(_doc))
                    return _doc;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public XmlDocument Action_LoadDOCFromDT(DataTable activeDT, string filerCondition, Dictionary<string, string> securitiesAttrs, bool isEncry)
        {
            XmlDocument _doc = new XmlDocument();
            _doc.LoadXml("<root></root>");
            Action_SigDoc(_doc);
            if (activeDT != null)
            {
                if (filerCondition == "")
                    return null;
                DataRow[] dr = null;
                if (filerCondition == "*")
                    dr = activeDT.Select();
                else
                    dr = activeDT.Select(filerCondition);
                if (dr.Length == 0)
                    return null;
                string result = "";
                if (securitiesAttrs.Count == 0 && dr.Length == 1)
                {
                    XmlNode newItem =  class_XmlHelper.CreateNode(_doc, "item", "");
                    _doc.SelectSingleNode("/root").AppendChild(newItem);
                    foreach (DataColumn dc in activeDT.Columns)
                    {
                        string columnValue = "";
                        class_Data_SqlDataHelper.StaticGetColumnData(dr[0], dc.ColumnName, out columnValue);
                        if (isEncry)
                            _des.DESCoding(columnValue, out result);
                        else
                            result = columnValue;
                        class_XmlHelper.SetAttribute(newItem, dc.ColumnName, result);
                    }
                }
                else if (securitiesAttrs.Count == 0 && dr.Length > 1)
                {
                    foreach (DataRow activeDR in dr)
                    {
                        XmlNode newItem = class_XmlHelper.CreateNode(_doc, "item", "");
                        _doc.SelectSingleNode("/root").AppendChild(newItem);
                        foreach (DataColumn dc in activeDT.Columns)
                        {
                            string columnValue = "";
                            class_Data_SqlDataHelper.StaticGetColumnData(activeDR, dc.ColumnName, out columnValue);
                            if (isEncry)
                                _des.DESCoding(columnValue, out result);
                            else
                                result = columnValue;
                            class_XmlHelper.SetAttribute(newItem, dc.ColumnName, result);
                        }
                    }
                }
                return _doc;
            }
            else
                return null;
        }        

        public bool Action_DoSaveDocFile(XmlDocument doc,string filename)
        {
            try
            {
                if (doc != null)
                {
                    doc.Save(filename);
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }   
    }

    public class Security_AuthorationAction
    {
        public bool Action_CheckWithAndLogicLink(XmlDocument securityDoc, Dictionary<string, string> checkingAttrMaping,string desKey)
        {
            if (checkingAttrMaping != null && checkingAttrMaping.Count > 0)
            {
                Security_DES _des = new Security_DES(desKey);
                bool resultFlag = false;
                string condition = "";
                foreach (string key in checkingAttrMaping.Keys)
                {
                    string value = "";                    
                    _des.DESCoding(checkingAttrMaping[key], out value);                   
                    condition = condition + "@" + key + "='" + value + "'" + " and ";
                }
                condition = condition.Remove(condition.Length - 5, 5);
                XmlNode activeNode = securityDoc.SelectSingleNode("/root/item[" + condition + "]");
                if (activeNode != null)
                    resultFlag = true;
                else
                    resultFlag = false;
                return resultFlag;
            }
            else
                return false;
        }

        public bool Action_Check(XmlDocument securityDoc, Dictionary<string, string> checkingAttrMaping,string desKey)
        {
            if (checkingAttrMaping != null && checkingAttrMaping.Count > 0)
            {
                bool resultFlag = false;
                Security_DES _des = new Security_DES(desKey);
                foreach (string key in checkingAttrMaping.Keys)
                {
                    string value = "";                   
                    _des.DESCoding(checkingAttrMaping[key], out value);
                    XmlNode activeNode = securityDoc.SelectSingleNode("/root/item[@" + key + "='" + value + "']");
                    if (activeNode != null)
                        resultFlag = true;
                    else
                        resultFlag = false;
                }
                return resultFlag;
            }
            else
                return false;
        }

        public bool Action_CheckCondition(XmlDocument securityDoc,Dictionary<string, string> checkingAttrMapingCondition, Dictionary<string, string> optionalConditionSymbol,string desKey)
        {
            if (checkingAttrMapingCondition != null && checkingAttrMapingCondition.Count > 0)
            {
                bool resultFlag = false;
                Security_DES _des = new Security_DES(desKey);
                foreach (string key in checkingAttrMapingCondition.Keys)
                {
                    string value = "";                    
                    _des.DESCoding(checkingAttrMapingCondition[key], out value);
                    XmlNode activeNode = securityDoc.SelectSingleNode("/root/item[@" + key + optionalConditionSymbol[key] + "'" + value + "']");
                    if (activeNode != null)
                        resultFlag = true;
                    else
                        resultFlag = false;
                }
                return resultFlag;
            }
            else
                return false;
        }
    }

}

