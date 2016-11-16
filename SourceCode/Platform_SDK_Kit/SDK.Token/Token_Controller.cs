﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace iKCoder_Platform_SDK_Kit.SDK.Token
{
    public class Token_Controller
    {
        System.Web.SessionState.HttpSessionState _refPageSession;
        
        public class_TokenItem registriedToken
        {
            set;
            get;
        }

        private Dictionary<string, class_TokenItem> _benchmarkTokensList = new Dictionary<string, class_TokenItem>();

        public bool AddBenchmarkToken(string name,string code,string key)
        {
            
            if (!_benchmarkTokensList.ContainsKey(name))
            {
                class_TokenItem newBenchmarkToken = new class_TokenItem();
                newBenchmarkToken.productName = name;
                string resultCode = code;
                newBenchmarkToken.productKey = key;
                newBenchmarkToken.productCode = resultCode;
                newBenchmarkToken.isBenchmark = true;
                _benchmarkTokensList.Add(name, newBenchmarkToken);
                return true;                
            }
            else
                return false;
        }

        public Token_Controller(System.Web.SessionState.HttpSessionState refPageSession)
        {
            _refPageSession = refPageSession;
        }
       
        public bool FlushToken(string tokenGuid)
        {
            if (string.IsNullOrEmpty(tokenGuid))
                return false;
            else
            {
                try
                {
                    if (_refPageSession[tokenGuid] != null)
                    {
                        class_TokenItem activeToken = (class_TokenItem)_refPageSession[tokenGuid];
                        activeToken.registryTime = DateTime.Now;
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
        
        public bool VerifyToken(string tokenGuid)
        {
            if (string.IsNullOrEmpty(tokenGuid))
                return false;
            else
            {
                try
                {
                    if (_refPageSession[tokenGuid] != null)
                    {
                        class_TokenItem activeToken = (class_TokenItem)_refPageSession[tokenGuid];
                        if((DateTime.Now-activeToken.registryTime).Minutes >= activeToken.expireMinutes)
                        {
                            _refPageSession.Remove(tokenGuid);
                            return false;
                        }
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

        public string GetToken(class_TokenItem activeToken)
        {
            if (activeToken != null)
            {
                if (_benchmarkTokensList.Count <= 0)
                    return string.Empty;
                else
                {
                    if (_benchmarkTokensList.ContainsKey(activeToken.productName))
                    {
                        string resuktDes = "";
                        class_Security_DES objectDes = new class_Security_DES(activeToken.productKey);
                        objectDes.DESDecoding(_benchmarkTokensList[activeToken.productName].productCode, out resuktDes);
                        if (_benchmarkTokensList[activeToken.productName].productCode == resuktDes)
                        {
                            string newGuid = "";
                            registriedToken = new class_TokenItem();
                            registriedToken.productName = activeToken.productName;
                            registriedToken.isBenchmark = false;
                            registriedToken.productCode = activeToken.productCode;
                            registriedToken.productKey = activeToken.productKey;
                            registriedToken.registryTime = DateTime.Now;
                            registriedToken.registryID = newGuid;
                            registriedToken.expireMinutes = activeToken.expireMinutes;
                            registriedToken.isBenchmark = false;
                            _refPageSession.Add(newGuid, registriedToken);
                            return newGuid;                            
                        }
                        else
                            return string.Empty;

                    }
                    else
                        return string.Empty;
                }
            }
            else
                return string.Empty;
        }

    }
}
