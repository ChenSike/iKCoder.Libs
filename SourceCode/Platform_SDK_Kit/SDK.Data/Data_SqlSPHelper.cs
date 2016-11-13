using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace iKCoder_Platform_SDK_Kit
{

    public class class_Data_SqlSPEntryNameFiler
    {
        public const string StartNamed_SelectAction = "Get";
        public const string StartNamed_Update = "Set";
        public const string StartNamed_All = "All";
    }

    public class class_Data_SqlSPEntryType
    {
        public const string UpdateAction = "Update";
        public const string SelectAction = "Select";
        public const string AllAction = "All";
    }

    public class class_Data_SqlSPEntry:ICloneable
    {
        public static bool AddSPEntryToQueue(string SPName, SqlDbType SPType, string EntryType, ref Queue<class_Data_SqlSPEntry> activeQueue)
        {
            if(activeQueue==null)
                return false;
            class_Data_SqlSPEntry activeEntry = new class_Data_SqlSPEntry();
            activeEntry.SPName = SPName;
            activeEntry.SPType = SPType;
            activeEntry.EntryType = EntryType;
            activeQueue.Enqueue(activeEntry);
            return true;
        }

        public static bool AddSPEntryToList(string SPName, SqlDbType SPType, string EntryType, ref List<class_Data_SqlSPEntry> activeList)
        {
            if(activeList==null)
                return false;
            class_Data_SqlSPEntry activeEntry = new class_Data_SqlSPEntry();
            activeEntry.SPName = SPName;
            activeEntry.SPType = SPType;
            activeEntry.EntryType = EntryType;
            activeList.Add(activeEntry);
            return true;
        }

        public static int GetSPParameterIndex(ref class_Data_SqlSPEntry activeSPEntry, string Paramername)
        {
            int index = 0;
            if (activeSPEntry != null)
            {
                foreach (SqlParameter activeParameter in activeSPEntry.ActiveParameters)
                {
                    if (activeParameter.ParameterName == Paramername)
                    {
                        return index;
                    }
                    index++;
                }
                return -1;
            }
            else
                return -1;
        }

        public static bool AddSPParameter(ref class_Data_SqlSPEntry activeSPEntry, string Paramername, SqlDbType SPType, ParameterDirection SPDirection,int size,object SPValue)
        {
            if (activeSPEntry == null)
                return false;
            SqlParameter activeParameter = new SqlParameter();            
            activeParameter.ParameterName = Paramername;
            activeParameter.Direction = SPDirection;
            activeParameter.SqlDbType = SPType;
            activeParameter.Value = SPValue;
            activeParameter.Size = size;
            activeSPEntry.ActiveParameters.Add(activeParameter);
            return true;
        }

        public static bool ModifySPParameter(ref class_Data_SqlSPEntry activeSPEntry, string Paramername, SqlDbType SPType, ParameterDirection SPDirection, object SPValue,int size=-1)
        {
            if (activeSPEntry == null)
                return false;
            else
            {
                SqlParameter activeParameter = null;
                foreach (SqlParameter tmpParameter in activeSPEntry.ActiveParameters)
                {
                    if (tmpParameter.ParameterName == Paramername)
                    {
                        activeParameter = tmpParameter;
                        break;
                    }
                }
                if (activeParameter != null)
                {
                    activeParameter.Direction = SPDirection;
                    activeParameter.SqlDbType = SPType;
                    activeParameter.Value = SPValue;
                    if (size != -1)
                        activeParameter.Size = size;
                    return true;
                }
                else
                    return false;                
            }
        }

        public static void ModifyExcludeSPParameter(ref class_Data_SqlSPEntry activeSPEntry, Dictionary<string,object> parameterMaping,List<string> filterParameter)
        {
            List<SqlParameter> excluded = new List<SqlParameter>();
            foreach (SqlParameter tmpParameter in activeSPEntry.ActiveParameters)
            {
                if (filterParameter.Contains(tmpParameter.ParameterName))
                    continue;
                if (parameterMaping.ContainsKey(tmpParameter.ParameterName))
                    continue;
                else
                {
                    excluded.Add(tmpParameter);
                }               
            }
            foreach (SqlParameter tmpExcludeParameter in excluded)
            {
                activeSPEntry.ActiveParameters.Remove(tmpExcludeParameter);
            }

        }
        
        public string SPName = "";
        public SqlDbType SPType = SqlDbType.NVarChar;
        public string KeyName = "";
        public string EntryType = "";
        public List<SqlParameter> ActiveParameters = new List<SqlParameter>();


        public object Clone()
        {
            class_Data_SqlSPEntry newEntry = new class_Data_SqlSPEntry();
            newEntry.ActiveParameters = this.ActiveParameters;
            newEntry.EntryType = this.EntryType;
            newEntry.KeyName = this.KeyName;
            newEntry.SPName = this.SPName;
            newEntry.SPType = this.SPType;
            return newEntry;
        }

        public static void CopyParams(ref class_Data_SqlSPEntry sourceEntry, ref class_Data_SqlSPEntry objectEntry)
        {
            foreach (SqlParameter activeParameter in sourceEntry.ActiveParameters)
            {
                SqlParameter cloneObj = new SqlParameter();
                cloneObj.ParameterName = activeParameter.ParameterName;
                cloneObj.Direction = activeParameter.Direction;
                cloneObj.DbType = activeParameter.DbType;
                objectEntry.ActiveParameters.Add(cloneObj);
            }
        }

        public static bool ClearAllParamersValue(ref List<SqlParameter> activeParamtersList)
        {
            if (activeParamtersList != null)
            {
                foreach (SqlParameter activeParameter in activeParamtersList)
                {
                    activeParameter.Value = null;
                }
                return true;
            }
            else
                return false;
        }

    }

    

    public class Data_SqlSPHelper
    {
        private class_Base_Config _refConfigObj;

        public Data_SqlSPHelper(class_Base_Config refConfigObj)
        {
            _refConfigObj = refConfigObj;
        }                       

        public Queue<class_Data_SqlSPEntry> Action_Auto_CreateNewQueue()
        {
            Queue<class_Data_SqlSPEntry> queue = new Queue<class_Data_SqlSPEntry>();
            if (_refConfigObj != null)
            {
                XmlNodeList activeItemNodes = _refConfigObj.GetItemNodes("storeProcedures", "spentry");
                foreach (XmlNode activeItemNode in activeItemNodes)
                {
                    class_Data_SqlSPEntry activeSPEntry = Action_GetSPConfig(activeItemNode);
                    if (activeSPEntry != null)
                        queue.Enqueue(activeSPEntry);

                }     
                return queue;
            }
            else
                return null;
        }

        public List<class_Data_SqlSPEntry> Action_Auto_CreateNewList()
        {
            List<class_Data_SqlSPEntry> list = new List<class_Data_SqlSPEntry>();
            if (_refConfigObj != null)
            {
                XmlNodeList activeItemNodes = _refConfigObj.GetItemNodes("storeProcedures", "spentry");
                foreach (XmlNode activeItemNode in activeItemNodes)
                {
                    class_Data_SqlSPEntry activeSPEntry = Action_GetSPConfig(activeItemNode);
                    if (activeSPEntry != null)
                        list.Add(activeSPEntry);
                }
                return list;
            }
            else
                return null;
        }

        public Dictionary<string, List<class_Data_SqlSPEntry>> Action_Auto_CreateNewDirc(string keyType)
        {
            Dictionary<string, List<class_Data_SqlSPEntry>> result = new Dictionary<string, List<class_Data_SqlSPEntry>>();
            if (_refConfigObj != null)
            {
                XmlNodeList activeItemNodes = _refConfigObj.GetItemNodes("storeProcedures", "spentry");
                List<class_Data_SqlSPEntry> _selectSP = new List<class_Data_SqlSPEntry>();
                List<class_Data_SqlSPEntry> _updateSP = new List<class_Data_SqlSPEntry>();
                List<class_Data_SqlSPEntry> keynameList = new List<class_Data_SqlSPEntry>();
                foreach (XmlNode activeItemNode in activeItemNodes)
                {
                    class_Data_SqlSPEntry activeSPEntry = Action_GetSPConfig(activeItemNode);
                    if (activeSPEntry != null)
                    {
                        switch (keyType)
                        {
                            case "keyname":
                            default:
                                keynameList.Add(activeSPEntry);
                                break;
                            case "entrytype":
                                if (activeSPEntry.EntryType == class_Data_SqlSPEntryType.SelectAction)
                                    _selectSP.Add(activeSPEntry);
                                else if (activeSPEntry.EntryType == class_Data_SqlSPEntryType.UpdateAction)
                                    _updateSP.Add(activeSPEntry);
                                break;

                        }
                    }
                }
                if (keyType == "keyname")
                    result.Add("keyname", keynameList);
                else if (keyType == "entrytype")
                {
                    result.Add(class_Data_SqlSPEntryType.SelectAction, _selectSP);
                    result.Add(class_Data_SqlSPEntryType.UpdateAction, _updateSP);
                }
                return result;
            }
            else
                return null;
        }

        public bool Action_CreateNewSPConfigDocument(string FilePath)
        {
            _refConfigObj = new class_Base_Config(FilePath);
            if (_refConfigObj != null)
            {
                if (_refConfigObj.CreateNewConfigDocument())
                {
                    _refConfigObj.CreateNewSession("documentType", "spconfig", false, "");
                    _refConfigObj.CreateNewSession("storeProcedures", "", false, "");
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

        }

        public SqlDbType Static_GetDbTypeFromConfigStr(string DBType)
        {
            switch (DBType)
            {
                case "SqlDbType.NVarChar":
                case "NVarChar":
                case "12":
                default:
                    return SqlDbType.NVarChar;
                case "SqlDbType.Int":
                case "Int":
                case "8":
                    return SqlDbType.Int;
                case "SqlDbType.Decimal":
                case "Decimal":
                case "5":
                    return SqlDbType.Decimal;
                case "SqlDbType.Binary":
                case "Binary":
                case "1":
                    return SqlDbType.Binary;
                case "SqlDbType.Image":
                case "Image":
                case "7":
                    return SqlDbType.Image;
                case "SqlDbType.DateTime":
                case "DateTime":
                case "4":
                    return SqlDbType.DateTime;

            }
        }

        public DbType Static_GetCommonDbTypeFromConfigStr(string DBType)
        {
            switch (DBType)
            {
                case "DbType.String":
                case "String":
                case "16":
                default:
                    return DbType.String;                    
                case "DbType.Int32":
                case "Int32":
                case "11":
                    return DbType.Int32;
                case "DbType.Decimal":
                case "Decimal":
                case "7":
                    return DbType.Decimal;
                case "DbType.Binary":
                case "Binary":
                case "1":
                    return DbType.Binary;
                case "DbType.DateTime":
                case "DateTime":
                case "6":
                    return DbType.DateTime;               
                    
            }
        }

        public ParameterDirection Static_GetDirectionFromConfigStr(string spDirection)
        {
            switch (spDirection)
            {
                case "1":
                case "Input":
                default:
                    return ParameterDirection.Input;
                case "2":
                case "Output":
                    return ParameterDirection.Output;
                case "3":
                case "InputOutput":
                    return ParameterDirection.InputOutput;
            }
        }

        public void Action_DoSaveAllConfigs()
        {
            _refConfigObj.DoSave();
        }

        public bool Action_CreateNewSPConfig(string SPKey, string SPName, ParameterDirection SPDirection, List<SqlParameter> paramList, string EntryType)
        {
            if (_refConfigObj != null)

            {
                if (Static_IsSPConfigDocument())
                {
                    XmlNode activeSessionNode = _refConfigObj.GetSessionNode("storeProcedures");
                    XmlNode activeConfigItemNode = _refConfigObj.CreateItem(activeSessionNode, "spentry", "", false, "");
                    _refConfigObj.SetItemAttr(activeConfigItemNode, "key", SPKey, false, "");
                    _refConfigObj.SetItemAttr(activeConfigItemNode, "spname", SPName, false, "");
                    _refConfigObj.SetItemAttr(activeConfigItemNode, "entrytype", EntryType, false, "");
                    foreach (SqlParameter parameter in paramList)
                    {
                        XmlNode parameterItemNode = _refConfigObj.CreateItem(activeConfigItemNode, "parameter", "", false,"");
                        _refConfigObj.SetItemAttr(parameterItemNode, "pname", parameter.ParameterName.ToString(), false, "");
                        _refConfigObj.SetItemAttr(parameterItemNode, "ptype", parameter.DbType.ToString(), false, "");
                        _refConfigObj.SetItemAttr(parameterItemNode, "pdirc", parameter.Direction.ToString(), false, "");
                        _refConfigObj.SetItemAttr(parameterItemNode, "pvalue", parameter.Value.ToString(), false, "");
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public class_Data_SqlSPEntry Action_GetSPConfig(XmlNode activeItemNode)
        {
            if (activeItemNode == null)
                return null;
            else
            {
                if (Static_IsSPConfigDocument())
                {
                    class_Data_SqlSPEntry activeEntry = new class_Data_SqlSPEntry();
                    string keyName = _refConfigObj.GetAttrValue(activeItemNode, "key", false, "");
                    string spName = _refConfigObj.GetAttrValue(activeItemNode, "spname", false, "");
                    string spentry = _refConfigObj.GetAttrValue(activeItemNode, "entrytype", false, "");
                    XmlNodeList parameterNodes = activeItemNode.SelectNodes("item[@name='parameter']");
                    foreach (XmlNode parameterNode in parameterNodes)
                    {
                        string parameterName = _refConfigObj.GetAttrValue(parameterNode, "pname", false, "");
                        string parameterType = _refConfigObj.GetAttrValue(parameterNode, "ptype", false, "");
                        string parameterDirc = _refConfigObj.GetAttrValue(parameterNode, "pdirc", false, "");
                        string parameterValue = _refConfigObj.GetAttrValue(parameterNode, "pvalue", false, "");
                        string parameterSize = _refConfigObj.GetAttrValue(parameterNode, "psize", false, "");
                        class_Data_SqlSPEntry.AddSPParameter(ref activeEntry, parameterName, Static_GetDbTypeFromConfigStr(parameterType), Static_GetDirectionFromConfigStr(parameterDirc), int.Parse(parameterSize), parameterValue);
                    }
                    return activeEntry;
                }
                else
                    return null;
            }
        }

        public class_Data_SqlSPEntry Action_GetSPConfig(string keyNameForSpConfig)
        {
            if (_refConfigObj != null)
            {
                if (Static_IsSPConfigDocument())
                {
                    XmlNode activeConfigItemNode = _refConfigObj.GetItemNode("storeProcedures", keyNameForSpConfig);
                    if (activeConfigItemNode != null)
                    {
                        class_Data_SqlSPEntry activeEntry = new class_Data_SqlSPEntry();
                        string keyName = _refConfigObj.GetAttrValue(activeConfigItemNode, "key", false, "");
                        string spName = _refConfigObj.GetAttrValue(activeConfigItemNode, "spname", false, "");
                        string spentry = _refConfigObj.GetAttrValue(activeConfigItemNode, "entrytype", false, "");
                        XmlNodeList parameterNodes = activeConfigItemNode.SelectNodes("item[@name='parameter']");
                        foreach (XmlNode parameterNode in parameterNodes)
                        {
                            string parameterName = _refConfigObj.GetAttrValue(parameterNode, "pname", false, "");
                            string parameterType = _refConfigObj.GetAttrValue(parameterNode, "ptype", false,"");
                            string parameterDirc = _refConfigObj.GetAttrValue(parameterNode, "pdirc", false,"");
                            string parameterValue = _refConfigObj.GetAttrValue(parameterNode, "pvalue", false, "");
                            string parameterSize = _refConfigObj.GetAttrValue(parameterNode, "psize", false, "");
                            class_Data_SqlSPEntry.AddSPParameter(ref activeEntry, parameterName, Static_GetDbTypeFromConfigStr(parameterType), Static_GetDirectionFromConfigStr(parameterDirc), int.Parse(parameterSize), parameterValue);                            
                        }
                        return activeEntry;
                    }
                    else
                        return null;

                }
                else
                    return null;
                
            }
            else
                return null;
        }

        public bool Static_IsSPConfigDocument()
        {
            if (_refConfigObj != null)
            {
                if (_refConfigObj.GetSessionValue("documentType", false, "") == "spconfig")
                    return true;
                else
                    return false;

            }
            else
                return false;
        }

    }
}
