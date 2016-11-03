using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKCoder_Platform_SDK_Kit
{
    public class Token_Status
    {
        public static string TOKEN_STATUS_CREATED_NEW = "TOKEN_CREATED_NEW";
        public static string TOKEN_STATUS_CREATED_RENEW = "TOKEN_CREATED_RENEW";
        public static string TOKEN_STATUS_CONNECTED_START = "TOKEN_CONNECTED_START";
        public static string TOKEN_STATUS_CONNECTED_CONNECTING = "TOKEN_CONNECTED_CONNECTING";
        public static string TOKEN_STATUS_CONNECTED_CONNECTED = "TOKEN_CONNECTED_CONNECTED";
        public static string TOKEN_STATUS_CONNECTED_RECONNECTED = "TOKEN_CONNECTED_RECONNECTED";
        public static string TOKEN_STATUS_CONNECTED_DISCONNECTED = "TOKEN_CONNECTED_DISCONNECTED";
        public static string TOKEN_STATUS_CONNECTED_OVERMAXCOUNT="TOKEN_CONNECTED_OVERMAXCOUNT";
        public static string TOKEN_STATUS_ACCESS_DENIFED = "TOKEN_ACCESS_DENIFIED";
        public static string TOKEN_STATUS_ACCESS_ACCEPTED = "TOKEN_ACCESS_ACCEPTED";
        public static string TOKEN_STATUS_ACCESS_WRONGID="TOKEN_ACCESS_WRONGID";
        public static string TOKEN_STATUS_ACCESS_WRONGCHECKCODE="TOKEN_ACCESS_WRONGCHECKCODE";
        public static string TOKEN_STATUS_RESPONSE_NORMAL="TOKEN_RESPONSE_NORMAL";
    }
}
