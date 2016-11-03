using System;
using System.Collections.Generic;

namespace iKCoder_Platform_SDK_Kit
{
    public class Base_AppExceptions : ApplicationException
    {
        public Base_AppExceptions()
        {
            LogServices_AppRuuningLogServices Log = new LogServices_AppRuuningLogServices(Environment.CurrentDirectory + @"\AppExceptionLog.xml");           
            Dictionary<string, string> attrList = new Dictionary<string, string>();
            attrList.Add("Catch_Time", DateTime.Now.ToString());
            attrList.Add("Message_Sys", base.Message);
            attrList.Add("Stack_Sys", base.StackTrace);
            Log.AddLogItem("Exception", attrList);
        }

        public void Write_Specil_Message(string Message)
        {
            LogServices_AppRuuningLogServices Log = new LogServices_AppRuuningLogServices(Environment.CurrentDirectory + @"\AppExceptionLog.xml");
            Dictionary<string, string> attrList = new Dictionary<string, string>();
            attrList.Add("Catch_Time", DateTime.Now.ToString());
            attrList.Add("Message_Sys", Message);
            attrList.Add("Stack_Sys", "None");
            Log.AddLogItem("Exception", attrList);
        }
    }
}

