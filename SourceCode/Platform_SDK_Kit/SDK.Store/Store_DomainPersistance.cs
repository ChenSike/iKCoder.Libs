using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKCoder_Platform_SDK_Kit
{

    public class Store_DomainPersistanceItem
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

    public class Store_DomainPersistance
    {
        
        public static Dictionary<string, Dictionary<string, Store_DomainPersistanceItem>> DataBuffer = new Dictionary<string, Dictionary<string, Store_DomainPersistanceItem>>();

        public void Add(string key, string domainKeyName,int storeExpeired,object data)
        {
            ClearBuffer();
            if (key != "")
            {
                Dictionary<string, Store_DomainPersistanceItem> activeStoreItem = null;
                if (!Store_DomainPersistance.DataBuffer.ContainsKey(key))
                {
                    activeStoreItem = new Dictionary<string, Store_DomainPersistanceItem>();
                    Store_DomainPersistance.DataBuffer.Add(key, activeStoreItem);
                }
                else
                    activeStoreItem = Store_DomainPersistance.DataBuffer[key];
                if (!activeStoreItem.ContainsKey(domainKeyName))
                {
                    Store_DomainPersistanceItem newItem = new Store_DomainPersistanceItem();
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

        public object Get(string key,string domainKeyName)
        {
            ClearBuffer();
            if (Store_DomainPersistance.DataBuffer.ContainsKey(key))
            {
                Dictionary<string, Store_DomainPersistanceItem> activeStoreItem = Store_DomainPersistance.DataBuffer[key];
                if (activeStoreItem.ContainsKey(domainKeyName))
                    return activeStoreItem[domainKeyName].Data;
                else
                    return null;
            }
            else
                return null;           
        }

        public void Remove(string key,string domainKeyName)
        {
            if (Store_DomainPersistance.DataBuffer.ContainsKey(key))
            {
                Dictionary<string, Store_DomainPersistanceItem> activeStoreItem = Store_DomainPersistance.DataBuffer[key];
                if (activeStoreItem.ContainsKey(domainKeyName))                
                    activeStoreItem.Remove(domainKeyName);                
            }
            ClearBuffer();
        }

        public void Flush(string key,string domainKeyName)
        {
            if (Store_DomainPersistance.DataBuffer.ContainsKey(key))
            {
                Dictionary<string, Store_DomainPersistanceItem> activeStoreItem = Store_DomainPersistance.DataBuffer[key];
                if (activeStoreItem.ContainsKey(domainKeyName))
                {
                    activeStoreItem[domainKeyName].Created = DateTime.Now;
                }
            }
            ClearBuffer();
        }

        public void ClearBuffer()
        {
            List<string> removedUrlLst =new List<string>();
            foreach(string activeUrl in DataBuffer.Keys)
            {
                List<string> removeLst = new List<string>();
                foreach(string keyName in DataBuffer[activeUrl].Keys)
                {                    
                    Store_DomainPersistanceItem tmpItem = DataBuffer[activeUrl][keyName];
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
}
