﻿using System;
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

        public string GetKeyName(string hostaddress,string producename)
        {
            return hostaddress + "_" + producename;
        }

        public void AddSingle(string key,string domainKeyName,int storeExpeired,object data)
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
            }
        }

        public void Add(string key, string domainKeyName,int storeExpeired,object data)
        {
            ClearBuffer();
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

        public object Get(string key,string domainKeyName)
        {
            ClearBuffer();
            if (DataBuffer.ContainsKey(key))
            {
                Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = DataBuffer[key];
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
            if (DataBuffer.ContainsKey(key))
            {
                Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = DataBuffer[key];
                if (activeStoreItem.ContainsKey(domainKeyName))                
                    activeStoreItem.Remove(domainKeyName);                
            }
            ClearBuffer();
        }

        public void Flush(string key,string domainKeyName)
        {
            if (DataBuffer.ContainsKey(key))
            {
                Dictionary<string, class_Store_DomainPersistanceItem> activeStoreItem = DataBuffer[key];
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
                    class_Store_DomainPersistanceItem tmpItem = DataBuffer[activeUrl][keyName];
                    if(tmpItem.Expeired == -999)                    
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

        public void ClearAll()
        {
            DataBuffer.Clear();            
        }
        
    }
}