using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


namespace iKCoder_Platform_SDK_Kit
{

    public class Token_Connector
    {               
        
        private string _TokenConnector_ID;
        private string _TokenConnector_CHECKCODE;
        private DateTime _TokenConnector_OBJCREATIONTIME;
        private string _TokenConnector_FROMIP;
        private string _TokenConnector_FROMNAME;
        private DateTime _TokenConnector_CONNECTEDTIME;
        private string _TokenConnector_Status;
        private string _TokenConnector_RequestURL;


        public Token_Connector()
        {
            _TokenConnector_OBJCREATIONTIME = DateTime.Now;
            string hostName = Dns.GetHostName();
            IPAddress[] IPAddress = Dns.GetHostAddresses(hostName);
            if (IPAddress.Length > 0)
                _TokenConnector_FROMIP = IPAddress[0].ToString();
            _TokenConnector_Status = Token_Status.TOKEN_STATUS_CREATED_NEW;
        }

        public Token_Connector(string TokenID,string TokenCheckCode)
        {
            _TokenConnector_OBJCREATIONTIME = DateTime.Now;
            string hostName = Dns.GetHostName();
            IPAddress[] IPAddress = Dns.GetHostAddresses(hostName);
            if (IPAddress.Length > 0)
                _TokenConnector_FROMIP = IPAddress[0].ToString();
            _TokenConnector_Status = Token_Status.TOKEN_STATUS_CREATED_NEW;
            _TokenConnector_ID = TokenID;
            _TokenConnector_CHECKCODE = TokenCheckCode;
        }

        public string GetCoderToken()
        {
            string result = Util_Common.SerializationObj(this);
            return result;
        }

        public static Token_Connector GetDecoderToken(string token)
        {
            return (Token_Connector)Util_Common.DeserializeObj(token);
        }
        
        public string TokenConector_ID
        {
            set
            {
                _TokenConnector_ID = value;
            }
            get
            {
                return _TokenConnector_ID;
            }
        }

        public string TokenConnector_CHECKCODE
        {
            set
            {
                _TokenConnector_CHECKCODE = value;
            }
            get
            {
                return _TokenConnector_CHECKCODE;
            }
        }

        public string TokenConnector_STATUS
        {
            set
            {
                _TokenConnector_Status = value;
            }
            get
            {
                return _TokenConnector_Status;
            }
        }        

        public string TokenConnector_FromIP
        {
            get
            {
                return _TokenConnector_FROMIP;
            }
        }

    }
}
