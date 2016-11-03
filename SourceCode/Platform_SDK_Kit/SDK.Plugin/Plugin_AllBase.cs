using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKCoder_Platform_SDK_Kit
{
    public class Plugin_AllBase
    {
        private string REGMODULEGUID = "";
        private TimeSpan TimeSpan_Running = new TimeSpan();

        public string ModuleID_Plugin
        {
            get
            {
                return REGMODULEGUID;
            }
        }

        public TimeSpan ModuleRunningTime_Plugin
        {
            get
            {
                return TimeSpan_Running;
            }
        }


        public Plugin_AllBase()
        {
            REGMODULEGUID = Guid.NewGuid().ToString();
        }

        public virtual void Action()
        {

        }

        public virtual object Action(Object Params)
        {
            return null;
        }
        
        public void Start()
        {
            DateTime dt_start = DateTime.Now;
            Action();
            DateTime dt_end = DateTime.Now;
            TimeSpan_Running = dt_end - dt_start;
        }
            

    }
}
