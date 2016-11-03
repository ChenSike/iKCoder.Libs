using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKCoder_Platform_SDK_Kit
{
    public interface Base_iInternalPlugin
    {
        object actionGet(object paramsList);
        object actionSet(object paramsList);
    }
}
