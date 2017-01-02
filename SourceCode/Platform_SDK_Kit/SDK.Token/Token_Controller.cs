using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Token_Controller
    {
        System.Web.SessionState.HttpSessionState _refPageSession;
        
        public class_TokenItem registriedToken
        {
            set;
            get;
        }

        private Dictionary<string, class_TokenItem> _benchmarkTokensList = new Dictionary<string, class_TokenItem>();

        public bool AddBenchmarkToken(string name,string code,int expired)
        {
            
            if (!_benchmarkTokensList.ContainsKey(name))
            {
                class_TokenItem newBenchmarkToken = new class_TokenItem();
                newBenchmarkToken.productName = name;
                string resultCode = code;
                newBenchmarkToken.productCode = resultCode;
                newBenchmarkToken.isBenchmark = true;
                newBenchmarkToken.expireMinutes = expired;
                _benchmarkTokensList.Add(name, newBenchmarkToken);
                return true;                
            }
            else
                return false;
        }

        public class_Token_Controller(System.Web.SessionState.HttpSessionState refPageSession)
        {
            _refPageSession = refPageSession;            
        }

        public class_TokenItem GetActiveToken(string tokenGuid)
        {
            if (!string.IsNullOrEmpty(tokenGuid))
            {
                if (_refPageSession[tokenGuid] != null)
                    return (class_TokenItem)_refPageSession[tokenGuid];
                else
                    return null;
            }
            else
                return null;
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
        
        public bool VerifyToken(string tokenGuid,out string fromProduct)
        {
            fromProduct = "";
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
                        fromProduct = activeToken.productName;
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
                        if (_benchmarkTokensList[activeToken.productName].productCode == activeToken.productCode)
                        {
                            string newGuid = "";
                            newGuid = Guid.NewGuid().ToString();
                            registriedToken = new class_TokenItem();
                            registriedToken.productName = activeToken.productName;
                            registriedToken.isBenchmark = false;
                            registriedToken.productCode = activeToken.productCode;
                            registriedToken.registryTime = DateTime.Now;
                            registriedToken.registryID = newGuid;
                            registriedToken.expireMinutes = _benchmarkTokensList[activeToken.productName].expireMinutes;
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
