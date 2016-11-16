using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_TokenItem
    {
   
        public class_TokenItem()
        {
            this.expireMinutes = 60;
        }

        public bool isBenchmark
        {
            set;
            get;
        }
        
        public string productName
        {
            set;
            get;
        }

        public string productCode
        {
            set;
            get;
        }

        public string productKey
        {
            set;
            get;
        }

        public string registryID
        {
            set;
            get;
        }

        public DateTime registryTime
        {
            set;
            get;
        }

        public int expireMinutes
        {
            set;
            get;
        }
        
    }
}
