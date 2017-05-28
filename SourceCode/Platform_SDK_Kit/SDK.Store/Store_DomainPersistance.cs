using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKCoder_Platform_SDK_Kit
{
        
    public class class_Store_DomainPersistanceItem
    {
        public string DomainKeyName
        {
            set;
            get;
        }

        public int Expeired
        {
            set;
            get;
        }

        public object Data
        {
            set;
            get;
        }

        public DateTime Created
        {
            set;
            get;
        }
       
    }

    public class class_Store_DomainPersistance
    {
        
        public Dictionary<string, Dictionary<string, class_Store_DomainPersistanceItem>> DataBuffer = new Dictionary<string, Dictionary<string, class_Store_DomainPersistanceItem>>();
               
        public class_Store_DomainPersistance()
        {            
            
        }

        public string GetKeyName(string sessionID)
        {
            /*
            if (clientsymbol != "")
                return hostaddress + "_" + producename + "_" + clientsymbol;
            else
                return hostaddress + "_" + producename;
                */
            return sessionID;
        }

        public string GetKeyName(string sessionID, string symbol)
        {
            return sessionID + "_" + symbol;
        }

        public void AddSingle(string key,string domainKeyName,int storeExpeired,object data)
        {
            if (key != "")
            {
                lock (DataBuffer)
                {
                    Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = null;
                    if (!DataBuffer.ContainsKey(key))
                    {
                        activeStoreItem = new Dictionary<string, class_Store_DomainPersistanceItem>();
                        DataBuffer.Add(key, activeStoreItem);
                    }
                    else
                        activeStoreItem = DataBuffer[key];
                    if (!activeStoreItem.ContainsKey(domainKeyName))
                    {
                        class_Store_DomainPersistanceItem newItem = new class_Store_DomainPersistanceItem();
                        newItem.DomainKeyName = domainKeyName;
                        newItem.Created = DateTime.Now;
                        newItem.Data = data;
                        newItem.Expeired = storeExpeired > 0 ? storeExpeired : 60;
                        activeStoreItem.Add(domainKeyName, newItem);
                    }
                }
            }
        }

        public void FlushValue(string key,string domainKeyName,object data)
        {
            lock (DataBuffer)
            {
                if (key != "")
                {
                    Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = null;
                    if (!DataBuffer.ContainsKey(key))
                    {
                        activeStoreItem = new Dictionary<string, class_Store_DomainPersistanceItem>();
                        DataBuffer.Add(key, activeStoreItem);
                    }
                    else
                        activeStoreItem = DataBuffer[key];
                    if (activeStoreItem.ContainsKey(domainKeyName))
                    {
                        activeStoreItem[domainKeyName].Data = data;
                        activeStoreItem[domainKeyName].Created = DateTime.Now;
                    }
                }
            }
        }

        public void Add(string key, string domainKeyName,int storeExpeired,object data)
        {
            ClearBuffer();
            lock (DataBuffer)
            {
                if (key != "")
                {
                    Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = null;
                    if (!DataBuffer.ContainsKey(key))
                    {
                        activeStoreItem = new Dictionary<string, class_Store_DomainPersistanceItem>();
                        DataBuffer.Add(key, activeStoreItem);
                    }
                    else
                        activeStoreItem = DataBuffer[key];
                    if (!activeStoreItem.ContainsKey(domainKeyName))
                    {
                        class_Store_DomainPersistanceItem newItem = new class_Store_DomainPersistanceItem();
                        newItem.DomainKeyName = domainKeyName;
                        newItem.Created = DateTime.Now;
                        newItem.Data = data;
                        newItem.Expeired = storeExpeired > 0 ? storeExpeired : 60;
                        activeStoreItem.Add(domainKeyName, newItem);
                    }
                    else
                    {
                        activeStoreItem[domainKeyName].DomainKeyName = domainKeyName;
                        activeStoreItem[domainKeyName].Data = data;
                        activeStoreItem[domainKeyName].Expeired = storeExpeired > 0 ? storeExpeired : 60;
                        activeStoreItem[domainKeyName].Created = DateTime.Now;
                    }
                }
            }
        }

        public object Get(string key,string domainKeyName)
        {
            lock (DataBuffer)
            {
                if (DataBuffer.ContainsKey(key))
                {
                    Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = DataBuffer[key];
                    object returnData = activeStoreItem[domainKeyName].Data;
                    if (activeStoreItem.ContainsKey(domainKeyName))
                    {
                        return returnData;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public void Remove(string key,string domainKeyName)
        {
            lock (DataBuffer)
            {
                if (DataBuffer.ContainsKey(key))
                {
                    Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = DataBuffer[key];
                    if (activeStoreItem.ContainsKey(domainKeyName))
                        activeStoreItem.Remove(domainKeyName);
                }
            }
            ClearBuffer();
        }

        public void Remove(string key)
        {
            lock (DataBuffer)
            {
                if (DataBuffer.ContainsKey(key))
                {
                    DataBuffer.Remove(key);
                }
            }
            ClearBuffer();
        }

        public void Flush(string key,string domainKeyName)
        {
            lock (DataBuffer)
            {
                if (DataBuffer.ContainsKey(key))
                {
                    Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = DataBuffer[key];
                    if (activeStoreItem.ContainsKey(domainKeyName))
                    {
                        activeStoreItem[domainKeyName].Created = DateTime.Now;
                    }
                }
            }
            ClearBuffer();
        }

        public bool IsKeyExisted(string KeyName)
        {
            lock(DataBuffer)
            {
                if (DataBuffer.ContainsKey(KeyName))
                    return true;
                else
                    return false;
            }
        }

        public bool IsDomainKeyExisted(string KeyName, string domainKeyName)
        {
            lock (DataBuffer)
            {
                if (DataBuffer.ContainsKey(KeyName))
                    if (DataBuffer[KeyName].ContainsKey(domainKeyName))
                        return true;
                    else
                        return false;
                else
                    return false;
            }
        }

        public void ClearBuffer()
        {
            lock (DataBuffer)
            {
                List<string> removedUrlLst = new List<string>();
                foreach (string activeUrl in DataBuffer.Keys)
                {
                    List<string> removeLst = new List<string>();
                    foreach (string keyName in DataBuffer[activeUrl].Keys)
                    {
                        class_Store_DomainPersistanceItem tmpItem = DataBuffer[activeUrl][keyName];
                        if (tmpItem.Expeired == 99999)
                            continue;
                        if ((DateTime.Now - tmpItem.Created).Minutes >= tmpItem.Expeired)
                            removeLst.Add(keyName);
                    }
                    foreach (string removeKeyName in removeLst)
                        DataBuffer[activeUrl].Remove(removeKeyName);
                    if (DataBuffer[activeUrl].Count == 0)
                        removedUrlLst.Add(activeUrl);
                }
                foreach (string activeRemovedUrl in removedUrlLst)
                    DataBuffer.Remove(activeRemovedUrl);
            }
        }

        public void ClearAll()
        {
            lock (DataBuffer)
            {
                DataBuffer.Clear();
            }
        }
        
    }
}
