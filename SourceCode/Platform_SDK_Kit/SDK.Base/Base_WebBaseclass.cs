using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;

namespace iKCoder_Platform_SDK_Kit
{
    public class Base_WebBaseclass:System.Web.UI.Page
    {

        protected XmlDocument RESPONSEDOCUMENT = new XmlDocument();
        public XmlDocument REQUESTDOCUMENT;        
        protected int REQUESTSPANTIME = 100;

        public string OVERREQUESTTIMEMSG
        {
            set;
            get;
        }
        
        public string APPFOLDERPATH
        {
            set;
            get;
        }

        public string REQUESTIP
        {
            set;
            get;
        }

        public Base_WebBaseclass()
        {
            RESPONSEDOCUMENT.LoadXml("<root></root>");
        }

        protected string GetQuerystringParam(string paramName)
        {
            if (paramName != "")
            {
                try
                {
                    return Request.QueryString[paramName].ToString();
                }
                catch
                {
                    return "";
                }
            }
            else
                return "";
        }       

        protected virtual void DoAction()
        {
           
        }

        protected void AddErrMessageToResponseDOC(string header, string message,string link)
        {
            XmlNode errNode = XmlHelper.CreateNode(RESPONSEDOCUMENT, "err", "");
            XmlHelper.SetAttribute(errNode, "header", header);
            XmlHelper.SetAttribute(errNode, "msg", message);
            XmlHelper.SetAttribute(errNode, "link", link);
            RESPONSEDOCUMENT.SelectSingleNode("/root").AppendChild(errNode);
        }

        protected void AddResponseMessageToResponseDOC(string header, string code, string message,string link)
        {
            XmlNode newNode = XmlHelper.CreateNode(RESPONSEDOCUMENT, "msg", "");
            XmlHelper.SetAttribute(newNode, "header", header);
            XmlHelper.SetAttribute(newNode, "code", code);
            XmlHelper.SetAttribute(newNode, "msg", message);
            XmlHelper.SetAttribute(newNode, "link", link);
            RESPONSEDOCUMENT.SelectSingleNode("/root").AppendChild(newNode);
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            REQUESTIP = Page.Request.UserHostAddress;
            if(REQUESTIP=="")
                REQUESTIP = "127.0.0.1";
            if(Session[REQUESTIP]!=null)
            {
                DateTime lastRequestTime =  DateTime.Now;
                if(Session[REQUESTIP].ToString()=="")
                {
                    DateTime.TryParse(Session[REQUESTIP].ToString(),out lastRequestTime);
                    if((lastRequestTime - DateTime.Now).Milliseconds < REQUESTSPANTIME)
                    {
                        
                    }
                }
            }
            Session[REQUESTIP] = DateTime.Now.ToString();
            if (Request.InputStream != null && Request.InputStream.Length > 0)
            {
                StreamReader streamReaderObj = new StreamReader(Request.InputStream);
                string requestStrDoc = streamReaderObj.ReadToEnd();
                streamReaderObj.Close();
                REQUESTDOCUMENT = new XmlDocument();
                REQUESTDOCUMENT.LoadXml(requestStrDoc);            
            }
            APPFOLDERPATH = Server.MapPath("~/");            
            DoAction();
        }       
    }
}
