using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using MySql.Data;
using MySql;
using MySql.Data.MySqlClient;

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

    public class class_data_MySqlSPEntry : class_Data_SqlSPEntry
    {
        public class_data_MySqlSPEntry(enum_DatabaseType activeDBType):base(activeDBType)
        {
            this.ActiveDBType = activeDBType;     
        }

        public MySqlDbType SPType
        {
            set;
            get;
        }
        
        public Dictionary<string, MySqlParameter> ParametersCollection = new Dictionary<string, MySqlParameter>();

        public static class_data_MySqlSPEntry CreateInstance(string SPName, MySqlDbType SPType, string EntryType,enum_DatabaseType activeDBType)
        {
            class_data_MySqlSPEntry newEntry = new class_data_MySqlSPEntry(activeDBType);
            newEntry.SPName = SPName;
            newEntry.SPType = SPType;
            newEntry.EntryType = EntryType;
            return newEntry;
        }             
       
        public List<MySqlParameter> GetActiveParametersList()
        {
            List<MySqlParameter> result = new List<MySqlParameter>();
            foreach (string parameterName in ParametersCollection.Keys)
                result.Add(ParametersCollection[parameterName]);
            return result;
        }

        public bool SetNewParameter(string Paraname, MySqlDbType SPType, ParameterDirection SPDirection,int size,object SPValue)
        {
            if (!ParametersCollection.ContainsKey(Paraname))
            {
                MySqlParameter activeParameter = new MySqlParameter();
                activeParameter.ParameterName = Paraname.StartsWith("@") ? Paraname.Replace("@", "_") : (Paraname.StartsWith("_") ? Paraname : "_" + Paraname);
                activeParameter.Direction = SPDirection;
                activeParameter.MySqlDbType = SPType;
                activeParameter.Value = SPValue;
                activeParameter.Size = size;
                ParametersCollection.Add(Paraname, activeParameter);
                return true;
            }
            else
                return false;
        }

        public override bool ModifyParameterValue(string Paraname,object SPValue)
        {
            Paraname = Paraname.StartsWith("@") ? Paraname.Replace("@", "_") : (Paraname.StartsWith("_") ? Paraname : "_" + Paraname);
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].Value = SPValue;
                ParametersCollection[Paraname].Size = SPValue.ToString().Length;
                return true;
            }
            else
                return false;
        }

        public bool ModifyParameterType(string Paraname, MySqlDbType SPType)
        {
            Paraname = Paraname.StartsWith("@") ? Paraname.Replace("@", "_") : (Paraname.StartsWith("_") ? Paraname : "_" + Paraname);
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].MySqlDbType = SPType;
                return true;
            }
            else
                return false;
        }

        public override bool ModifyParameterDirection(string Paraname,ParameterDirection SPDirection)
        {
            Paraname = Paraname.StartsWith("@") ? Paraname.Replace("@", "_") : (Paraname.StartsWith("_") ? Paraname : "_" + Paraname);
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].Direction = SPDirection;
                return true;
            }
            else
                return false;
        }

        public override bool ModifyParameterSize(string Paraname,int SPSize)
        {
            Paraname = Paraname.StartsWith("@") ? Paraname.Replace("@", "_") : (Paraname.StartsWith("_") ? Paraname : "_" + Paraname);
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
            class_data_MySqlSPEntry newEntry = new class_data_MySqlSPEntry(this.ActiveDBType);
            newEntry.ParametersCollection = this.ParametersCollection;
            newEntry.EntryType = this.EntryType;
            newEntry.KeyName = this.KeyName;
            newEntry.SPName = this.SPName;
            newEntry.SPType = this.SPType;
            return newEntry;
        }

        public override void ClearAllParams()
        {
            this.ParametersCollection.Clear();
        }

        public override void ClearAllParamsValues()
        {
            foreach (string paramName in ParametersCollection.Keys)
            {
                this.ParametersCollection[paramName].Value = null;
            }
        }

        public bool RemoveParam(string Paraname)
        {
            Paraname = Paraname.StartsWith("@") ? Paraname.Replace("@", "_") : (Paraname.StartsWith("_") ? Paraname : "_" + Paraname);
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection.Remove(Paraname);
                return true;
            }
            else
                return false;
        }
    }

    public class class_data_SqlServerSPEntry : class_Data_SqlSPEntry
    {
        public class_data_SqlServerSPEntry(enum_DatabaseType activeDBType):base(activeDBType)
        {
            this.ActiveDBType = activeDBType;     
        }

        public SqlDbType SPType
        {
            set;
            get;
        }
        
        public Dictionary<string, SqlParameter> ParametersCollection = new Dictionary<string, SqlParameter>();

        public static class_data_SqlServerSPEntry CreateInstance(string SPName, SqlDbType SPType, string EntryType,enum_DatabaseType activeDBType)
        {
            class_data_SqlServerSPEntry newEntry = new class_data_SqlServerSPEntry(activeDBType);
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
                activeParameter.ParameterName = Paraname.StartsWith("@") ? Paraname : "@" + Paraname;                
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

        public override bool ModifyParameterValue(string Paraname,object SPValue)
        {
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].Value = SPValue;
                ParametersCollection[Paraname].Size = SPValue.ToString().Length;
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

        public override bool ModifyParameterDirection(string Paraname,ParameterDirection SPDirection)
        {
            if (ParametersCollection.ContainsKey(Paraname))
            {
                ParametersCollection[Paraname].Direction = SPDirection;
                return true;
            }
            else
                return false;
        }

        public override bool ModifyParameterSize(string Paraname,int SPSize)
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
            class_data_SqlServerSPEntry newEntry = new class_data_SqlServerSPEntry(this.ActiveDBType);
            newEntry.ParametersCollection = this.ParametersCollection;
            newEntry.EntryType = this.EntryType;
            newEntry.KeyName = this.KeyName;
            newEntry.SPName = this.SPName;
            newEntry.SPType = this.SPType;
            return newEntry;
        }

        public override void ClearAllParams()
        {
            this.ParametersCollection.Clear();
        }

        public override void ClearAllParamsValues()
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
