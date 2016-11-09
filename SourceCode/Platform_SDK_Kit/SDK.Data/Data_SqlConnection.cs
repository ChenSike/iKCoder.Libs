using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace iKCoder_Platform_SDK_Kit
{

    public class Data_SqlConnectionItemEntry
    {
        public string Key;
        public string Server;
        public string UserID;
        public string Password;
        public string ActiveDataBase;
        public string ConnectionString = "server={server};uid={uid};pwd={pwd};database={db}";
        public SqlConnection ActiveConnection;
    }

    public class Data_SqlConnectionHelper
    {        
        public Dictionary<string, Data_SqlConnectionItemEntry> ActiveSqlConnectionCollection = new Dictionary<string, Data_SqlConnectionItemEntry>();

        public bool Set_NewConnectionItem(string Key, string Server, string UserID, string Password,string activeDB)
        {
            if (!ActiveSqlConnectionCollection.ContainsKey(Key))
            {
                Data_SqlConnectionItemEntry newEntry = new Data_SqlConnectionItemEntry();
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
                catch(class_Base_AppExceptions err)
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
                    {
                        ActiveSqlConnectionCollection[key].ActiveConnection.Close();
                        ActiveSqlConnectionCollection.Remove(key);
                    }
                }
                catch(class_Base_AppExceptions err)
                {
                    continue;
                }
            }
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
