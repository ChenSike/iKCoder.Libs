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
        public const string InsertAction = "Insert";
        public const string DeleteAction = "Delete";
        public const string AllAction = "All";
    }

    public class class_Data_SqlSPEntry:ICloneable
    {

        public class_Data_SqlSPEntry(enum_DatabaseType activeDBType)
        {
            this.ActiveDBType = activeDBType;
        }

        public enum_DatabaseType ActiveDBType
        {
            set;
            get;
        }

        public string SPName
        {
            set;
            get;
        }

        public SqlDbType SPType
        {
            set;
            get;
        }

        public string KeyName
        {
            set;
            get;
        }

        public string EntryType
        {
            set;
            get;
        }

        public Dictionary<string, SqlParameter> ParametersCollection = new Dictionary<string, SqlParameter>();

        public static class_Data_SqlSPEntry CreateInstance(string SPName, SqlDbType SPType, string EntryType,enum_DatabaseType activeDBType)
        {
            class_Data_SqlSPEntry newEntry = new class_Data_SqlSPEntry(activeDBType);
            newEntry.SPName = SPName;
            newEntry.SPType = SPType;
            newEntry.EntryType = EntryType;
            return newEntry;
        }              
       

        public List<SqlParameter> GetActiveParametersList()
        {
            List<SqlParameter> result = new List<SqlParameter>();
            foreach (string parameterName in ParametersCollection.Keys)
                result.Add(ParametersCollection[parameterName]);
            return result;
        }

        public bool SetNewParameter(string Paraname, SqlDbType SPType, ParameterDirection SPDirection,int size,object SPValue)
        {
            if (!ParametersCollection.ContainsKey(Paraname))
            {
                SqlParameter activeParameter = new SqlParameter();
                if (ActiveDBType == enum_DatabaseType.SqlServer)
                    activeParameter.ParameterName = Paraname.StartsWith("@") ? Paraname : "@" + Paraname;
                else if (ActiveDBType == enum_DatabaseType.MySql)
                    activeParameter.ParameterName = Paraname.StartsWith("@") ? Paraname.Replace("@", "_") : (Paraname.StartsWith("_") ? Paraname : "_" + Paraname);
                activeParameter.Direction = SPDirection;
                activeParameter.SqlDbType = SPType;
                activeParameter.Value = SPValue;
                activeParameter.Size = size;
                ParametersCollection.Add(Paraname, activeParameter);
                return true;
            }
            else
                return false;
        }

        public bool ModifyParameterValue(string Paraname,object SPValue)
        {
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].Value = SPValue;
                return true;
            }
            else
                return false;
        }

        public bool ModifyParameterType(string Paraname, SqlDbType SPType)
        {
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].SqlDbType = SPType;
                return true;
            }
            else
                return false;
        }

        public bool ModifyParameterDirection(string Paraname,ParameterDirection SPDirection)
        {
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].Direction = SPDirection;
                return true;
            }
            else
                return false;
        }

        public bool ModifyParameterSize(string Paraname,int SPSize)
        {
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].Size = SPSize;
                return true;
            }
            else
                return false;
        }
            
        public object Clone()
        {
            class_Data_SqlSPEntry newEntry = new class_Data_SqlSPEntry(this.ActiveDBType);
            newEntry.ParametersCollection = this.ParametersCollection;
            newEntry.EntryType = this.EntryType;
            newEntry.KeyName = this.KeyName;
            newEntry.SPName = this.SPName;
            newEntry.SPType = this.SPType;
            return newEntry;
        }

        public void ClearAllParams()
        {
            this.ParametersCollection.Clear();
        }

        public void ClearAllParamsValues()
        {
            foreach (string paramName in ParametersCollection.Keys)
            {
                this.ParametersCollection[paramName].Value = string.Empty;
            }
        }

        public bool RemoveParam(string Paraname)
        {
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection.Remove(Paraname);
                return true;
            }
            else
                return false;
        }

    }

}
