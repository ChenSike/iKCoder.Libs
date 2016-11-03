using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iKCoder_Platform_SDK_Kit
{
    public class Pools_Security
    {
        public static Dictionary<string, Security_UserItem> ActiveUsers = new Dictionary<string, Security_UserItem>();
        public static Dictionary<string, Security_UserGroup> ActiveGroups = new Dictionary<string, Security_UserGroup>();
        public static Dictionary<string, Security_AuthoriationContext> ActiveContexts = new Dictionary<string, Security_AuthoriationContext>();

        public static bool AddItemToContextlist(Security_AuthoriationContext activeContextItem)
        {
            if (activeContextItem != null)
            {
                if (!ActiveContexts.ContainsKey(activeContextItem.strRemoteGUID))
                {
                    ActiveContexts.Add(activeContextItem.strRemoteGUID, activeContextItem);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }        

        public static bool AddItemToUserlist(Security_UserItem activeUserItem)
        {
            if (activeUserItem != null)
            {
                if (!ActiveUsers.ContainsKey(activeUserItem.UserName))
                {
                    ActiveUsers.Add(activeUserItem.UserName, activeUserItem);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static bool AddItemToGrouplist(Security_UserGroup activeGroupItem)
        {
            if (activeGroupItem != null)
            {
                if (!ActiveGroups.ContainsKey(activeGroupItem.GroupKeyName))
                {
                    ActiveGroups.Add(activeGroupItem.GroupKeyName,activeGroupItem);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static Security_UserItem GetItemFromUserlist(string userName)
        {
            if (userName != "")
            {
                if (ActiveUsers.ContainsKey(userName))
                    return ActiveUsers[userName];
                else
                    return null;
            }
            else
                return null;
        }

        public static Security_UserGroup GetItemFromGrouplist(string groupName)
        {
            if (groupName != "")
            {
                if (ActiveGroups.ContainsKey(groupName))
                    return ActiveGroups[groupName];
                else
                    return null;
            }
            else
                return null;
        }

    }
}
