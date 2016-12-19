using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Data_SqlConnectionItemEntry
    {
        public string Key
        {
            set;
            get;
        }

        public string Server
        {
            set;
            get;
        }

        public string UserID
        {
            set;
            get;
        }

        public string Password
        {
            set;
            get;
        }

        public string ActiveDataBase;
        public string ConnectionString = "server={server};uid={uid};pwd={pwd};database={db}";

        public void CreateNewConnectionObject(enum_SqlConnectionType activeType)
        {
            this.ActiveConnection.activeConnectionType = activeType;

        }

        protected Data_PlatformDBConnection ActiveConnection
        {
            set;
            get;
        }

    }   

    public class class_Data_SqlConnectionHelper
    {        
        public Dictionary<string, class_Data_SqlConnectionItemEntry> ActiveSqlConnectionCollection = new Dictionary<string, class_Data_SqlConnectionItemEntry>();

        public bool Set_NewConnectionItem(string Key, string Server, string UserID, string Password,string activeDB)
        {
            if (!ActiveSqlConnectionCollection.ContainsKey(Key))
            {
                class_Data_SqlConnectionItemEntry newEntry = new class_Data_SqlConnectionItemEntry(); 
                newEntry.Server = Server;
                newEntry.UserID = UserID;
                newEntry.Password = Password;
                newEntry.ActiveDataBase = activeDB;
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{server}", Server);
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{uid}", UserID);
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{pwd}", Password);
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{db}", activeDB);
                newEntry.ActiveConnection = new SqlConnection(newEntry.ConnectionString);
                try
                {
                    newEntry.ActiveConnection.Open();
                    ActiveSqlConnectionCollection.Add(Key, newEntry);
                    return true;
                }
                catch
                {                    
                    return false;
                }
            }
            else
                return false;
        }
        
        public SqlConnection Get_ActiveConnection(string key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(key))
                return ActiveSqlConnectionCollection[key].ActiveConnection;
            else
                return null;
        }

        public void Action_CloseAllActionConnection()
        {
            foreach (string key in ActiveSqlConnectionCollection.Keys)
            {
                try
                {
                    if (ActiveSqlConnectionCollection[key].ActiveConnection.State != ConnectionState.Closed)                    
                        ActiveSqlConnectionCollection[key].ActiveConnection.Close();                                            
                }
                catch(class_Base_AppExceptions err)
                {
                    continue;
                }
            }
            ActiveSqlConnectionCollection.Clear();
        }

        public bool Active_CloseActiveConnection(string Key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(Key))
            {
                ActiveSqlConnectionCollection[Key].ActiveConnection.Close();
                ActiveSqlConnectionCollection.Remove(Key);
                return true;
            }
            else
                return false;
        }

    }
}
