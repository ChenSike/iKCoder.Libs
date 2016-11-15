using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKCoder_Platform_SDK_Kit.SDK.Token
{
    public class Token_Controller
    {
        public Dictionary<string, class_TokenItem> registryTokensPool
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

        public Token_Controller(Dictionary<string,class_TokenItem> activeRegistryTokensPool)
        {
            this.registryTokensPool = activeRegistryTokensPool;
        }        

        public string VerifyToken(class_TokenItem activeToken)
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
                            while (registryTokensPool.ContainsKey(newGuid))
                                 newGuid = Guid.NewGuid().ToString();
                            class_TokenItem newRegItem = new class_TokenItem();
                            newRegItem.productName = activeToken.productName;
                            newRegItem.isBenchmark = false;
                            newRegItem.productCode = activeToken.productCode;
                            newRegItem.productKey = activeToken.productKey;
                            newRegItem.registryTime = DateTime.Now.ToString();
                            newRegItem.registryID = newGuid;
                            //registryTokensPool.Add(" ",)
                            
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
