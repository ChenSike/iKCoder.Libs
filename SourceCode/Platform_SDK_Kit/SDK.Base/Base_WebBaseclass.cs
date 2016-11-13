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
    public class class_Base_WebBaseclass:System.Web.UI.Page
    {

        protected XmlDocument RESPONSEDOCUMENT = new XmlDocument();
        public XmlDocument REQUESTDOCUMENT;        
        protected int REQUESTSPANTIME = 100;
        protected string OVERREQUESTTIME = "<root><msg>You are sending request to API server too often,please visit later.</msg></root>";
        
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

        public class_Base_WebBaseclass()
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

        protected virtual void BeforeLoad()
        {

        }

        protected virtual void DoAction()
        {
           
        }

        protected void AddErrMessageToResponseDOC(string header, string message,string link)
        {
            XmlNode errNode = class_XmlHelper.CreateNode(RESPONSEDOCUMENT, "err", "");
            class_XmlHelper.SetAttribute(errNode, "header", header);
            class_XmlHelper.SetAttribute(errNode, "msg", message);
            class_XmlHelper.SetAttribute(errNode, "link", link);
            RESPONSEDOCUMENT.SelectSingleNode("/root").AppendChild(errNode);
        }

        protected void AddResponseMessageToResponseDOC(string header, string code, string message,string link)
        {
            XmlNode newNode = class_XmlHelper.CreateNode(RESPONSEDOCUMENT, "msg", "");
            class_XmlHelper.SetAttribute(newNode, "header", header);
            class_XmlHelper.SetAttribute(newNode, "code", code);
            class_XmlHelper.SetAttribute(newNode, "msg", message);
            class_XmlHelper.SetAttribute(newNode, "link", link);
            RESPONSEDOCUMENT.SelectSingleNode("/root").AppendChild(newNode);
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            REQUESTIP = Page.Request.UserHostAddress;
            if(string.IsNullOrEmpty(REQUESTIP))
                REQUESTIP = "127.0.0.1";
            if(Session[REQUESTIP]!=null)
            {
                DateTime lastRequestTime =  DateTime.Now;
                if(Session[REQUESTIP].ToString()=="")
                {
                    DateTime.TryParse(Session[REQUESTIP].ToString(),out lastRequestTime);
                    if((lastRequestTime - DateTime.Now).Milliseconds < REQUESTSPANTIME)
                    {
                        RESPONSEDOCUMENT.LoadXml(OVERREQUESTTIME);
                        Response.Write(RESPONSEDOCUMENT.OuterXml);
                        return;
                    }
                }
                Session[REQUESTIP] = DateTime.Now.ToString();
            }
            else            
                Session.Add(REQUESTIP, DateTime.Now.ToString());              
            if (Request.InputStream != null && Request.InputStream.Length > 0)
            {
                StreamReader streamReaderObj = new StreamReader(Request.InputStream);
                string requestStrDoc = streamReaderObj.ReadToEnd();
                streamReaderObj.Close();
                REQUESTDOCUMENT = new XmlDocument();
                REQUESTDOCUMENT.LoadXml(requestStrDoc);            
            }
            APPFOLDERPATH = Server.MapPath("~/");
            BeforeLoad();
            DoAction();
        }       
    }
}
