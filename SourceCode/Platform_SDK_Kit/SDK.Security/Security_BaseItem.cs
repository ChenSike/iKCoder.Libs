using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace iKCoder_Platform_SDK_Kit
{
    public class Security_BaseItem
    {
       protected class_Security_DES Security_DESObj;
    }

    public enum Security_ActionType
    {
        Web_Action,
        Form_Action
    }

    public class Security_ActionItem
    {
        public Security_ActionType ActionType;
        public string Action;
    }

    public class Security_UserGroup : Security_BaseItem
    {

        private string Group_KeyName;
        private string Group_Key;         
        public Dictionary<string, Security_ActionItem> Group_Actions = new Dictionary<string, Security_ActionItem>();        

        public Security_UserGroup(string GroupKey)
        {
            this.Group_Key = GroupKey;
            Security_DESObj=new class_Security_DES(GroupKey);
        }

        public string GroupKey
        {
            set
            {
                this.Group_Key = value;
                Security_DESObj = new class_Security_DES(this.Group_Key);
            }
        }

        public string GroupKeyName
        {
            set
            {
                string result = "";
                Security_DESObj.DESCoding(value, out result);
                this.Group_KeyName = result;
            }
            get
            {
                string result = "";
                Security_DESObj.DESDecoding(this.Group_KeyName, out result);
                return result;
            }
        }
    }

    public class Security_UserItem : Security_BaseItem
    {
        private string User_Name;
        private string User_Password;
        private string User_Key;
        private bool User_Locked = false;
        private List<string> User_GroupKeyList = new List<string>();        

        public Security_UserItem(string UserKey)
        {
            this.User_Key = UserKey;
            Security_DESObj = new class_Security_DES(this.User_Key);
        }

        public string UserName
        {
            set
            {
                if (value != "")
                {
                    string result = "";
                    Security_DESObj.DESCoding(value, out result);
                    this.User_Name = result;
                }
            }
            get
            {
                string result = "";
                Security_DESObj.DESDecoding(this.User_Name, out result);
                return result;
            }
        }

        public string UserKey
        {
            set
            {
                this.User_Key = value;
                Security_DESObj = new class_Security_DES(this.User_Key);
            }
            get
            {
                return this.User_Key;
            }
        }

        public string UserPassword
        {
            set
            {
                string result = "";
                Security_DESObj.DESCoding(value,out result);
                this.User_Password = result;
            }
            get
            {
                string result = "";
                Security_DESObj.DESDecoding(this.User_Password, out result);
                return result;
            }
        }

        public bool Action_InsertGroup(string GroupName)
        {
            if (GroupName != "")
            {
                if (!User_GroupKeyList.Contains(GroupName))
                    User_GroupKeyList.Add(GroupName);
                return true;
            }
            else
                return false;
        }

        public bool Action_DelGroup(string GroupName)
        {
            if (GroupName != "")
            {
                if (User_GroupKeyList.Contains(GroupName))
                    User_GroupKeyList.Remove(GroupName);
                return true;
            }
            else
                return false;
        }


        public List<string> Action_GetGroup()
        {
            return User_GroupKeyList;
        }       

    }

    public class Security_AuthoriationContext : Security_BaseItem
    {
        public bool isAllowed = false;
        public string strRemoteGUID = "";
        public string strRemoteIP = "";
        
        public Security_AuthoriationContext(string contextKey)
        {
            Security_DESObj = new class_Security_DES(contextKey);
        }


    }


}
