﻿using System;
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
        protected byte[] RESPONSEBUFFER;
        public XmlDocument REQUESTDOCUMENT;
        public string REQUESTCONTENT = string.Empty; 
        protected int REQUESTSPANTIME = 2;        
        protected bool ISRESPONSEDOC = true;
        protected bool ISBINRESPONSE = false;
        protected bool ISTEXTREQUEST = false;
        protected static class_Store_DomainPersistance Object_DomainPersistance = new class_Store_DomainPersistance();
                
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

        public string GetClientIPAddr()
        {
            /*
            string userHostAddress = "";
            if (HttpContext.Current.Request.ServerVariables.AllKeys.Contains("HTTP_X_FORWARDED_FOR"))
            {
                userHostAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.UserHostAddress;
            }
            if (!string.IsNullOrEmpty(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
            */
            return Session.SessionID;

        }

        public string ClientSymbol
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

        protected object GetSessionValue(string sessionName)
        {
            if (Session[sessionName] != null)
                return Session[sessionName];
            else
                return null;
        }

        protected object GetApplicationData(string applicationName)
        {
            if (Application[applicationName] != null)
                return Application[applicationName];
            else
                return null;
        }

        protected string GetSessionKeyName(string product,string sessionName)
        {
            return product + "_" + sessionName;
        }

        protected HttpCookie GetRequestCookie(string cookieName)
        {
            HttpCookie activeCookie = Request.Cookies.Get(cookieName);
            if (activeCookie != null)
                return activeCookie;
            else
                return null;
        }

        protected virtual void BeforeLoad()
        {

        }

        protected virtual void DoAction()
        {
           
        }

        protected virtual void InitAction()
        {

        }

        protected virtual void CheckRegDomain()
        {

        }

        public string RSDoamin
        {
            set;
            get;
        }

        protected enum enumResponseMode
        {
            bin = 1,
            text = 2            
        }
       

        protected void AddErrMessageToResponseDOC(string header, string message,string link,enum_MessageType activeMessageType = enum_MessageType.Message)
        {
            XmlNode errNode = class_XmlHelper.CreateNode(RESPONSEDOCUMENT, "err", "");
            class_XmlHelper.SetAttribute(errNode, "header", header);
            class_XmlHelper.SetAttribute(errNode, "msg", message);
            class_XmlHelper.SetAttribute(errNode, "link", link);
            RESPONSEDOCUMENT.SelectSingleNode("/root").AppendChild(errNode);
        }

        protected void AddErrMessageToResponseDOC(string header, string message, Dictionary<string, string> attrsList, enum_MessageType activeMessageType = enum_MessageType.Message)
        {
            XmlNode errNode = class_XmlHelper.CreateNode(RESPONSEDOCUMENT, "err", "");
            class_XmlHelper.SetAttribute(errNode, "header", header);
            class_XmlHelper.SetAttribute(errNode, "msg", message);
            foreach (string attrName in attrsList.Keys)
                class_XmlHelper.SetAttribute(errNode, attrName, attrsList[attrName]);
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

        protected void AddResponseMessageToResponseDOC(string header, string code,Dictionary<string,string> attrsList)
        {
            XmlNode newNode = class_XmlHelper.CreateNode(RESPONSEDOCUMENT, "msg", "");
            class_XmlHelper.SetAttribute(newNode, "header", header);
            class_XmlHelper.SetAttribute(newNode, "code", code);
            foreach(string attrName in attrsList.Keys)            
                class_XmlHelper.SetAttribute(newNode, attrName, attrsList[attrName]);  
            RESPONSEDOCUMENT.SelectSingleNode("/root").AppendChild(newNode);
        }

        protected void switchResponseMode(enumResponseMode activeResponseMode)
        {
            switch(activeResponseMode)
            {
                case enumResponseMode.bin:
                    ISBINRESPONSE = true;
                    ISRESPONSEDOC = false;
                    break;
                case enumResponseMode.text:
                    ISBINRESPONSE = false;
                    ISRESPONSEDOC = true;
                    break;                
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.REQUESTIP = GetClientIPAddr();
            if (string.IsNullOrEmpty(this.REQUESTIP))
                this.REQUESTIP = Guid.NewGuid().ToString();
            if (Session[REQUESTIP] != null)
            {
                if (Session[REQUESTIP].ToString() == "")
                {
                    DateTime lastRequestTime = DateTime.Now;
                    DateTime.TryParse(Session[REQUESTIP].ToString(), out lastRequestTime);
                    if ((lastRequestTime - DateTime.Now).Milliseconds < REQUESTSPANTIME)
                    {
                        AddErrMessageToResponseDOC("Request too often from client", "Sending request document too ofter.", "");
                        Response.Write(RESPONSEDOCUMENT.OuterXml);
                        return;
                    }
                }
                Session[REQUESTIP] = DateTime.Now.ToString();
            }
            else
                Session.Add(REQUESTIP, DateTime.Now.ToString());
            APPFOLDERPATH = Server.MapPath("~/");         
            ClientSymbol = GetQuerystringParam("cid");
            ISTEXTREQUEST = GetQuerystringParam("istextreq") == "1" ? true : false;
            if (string.IsNullOrEmpty(ClientSymbol))
            {
                if (Session["ClientSymbol"] == null)
                {
                    ClientSymbol = Guid.NewGuid().ToString();
                    Session.Add("ClientSymbol", ClientSymbol);
                }
                else
                    ClientSymbol = Session["ClientSymbol"].ToString();
            }

            InitAction();                         
            BeforeLoad();
            CheckRegDomain();

            bool isSumitData = GetQuerystringParam("sumitdata") == "1" ? true : false;
            if (Request.InputStream != null && Request.InputStream.Length > 0)
            {
                if (!isSumitData)
                {
                    StreamReader streamReaderObj = new StreamReader(Request.InputStream);
                    string requestStrDoc = streamReaderObj.ReadToEnd();
                    streamReaderObj.Close();
                    try
                    {
                        if (ISTEXTREQUEST)
                            REQUESTCONTENT = requestStrDoc;
                        else
                        {
                            REQUESTDOCUMENT = new XmlDocument();
                            REQUESTDOCUMENT.LoadXml(requestStrDoc);
                        }
                    }
                    catch
                    {
                        REQUESTCONTENT = requestStrDoc;
                    }
                }                
            }
            if (!string.IsNullOrEmpty(RSDoamin))
            {
                Response.AddHeader("Access-Control-Allow-Credentials", "true");
                Response.AddHeader("Access-Control-Allow-Headers", "Content-Type,x-requested-with");
                Response.AddHeader("Access-Control-Allow-Methods", "POST,GET,OPTIONS,DELETE");
                Response.AddHeader("Access-Control-Allow-Origin", this.RSDoamin);                          
            }           
            DoAction();
            if (ISRESPONSEDOC)
            {
                Response.ContentType = "text/xml; characterset=utf-8";
                Response.Write(RESPONSEDOCUMENT.OuterXml);
            }
            else if (ISBINRESPONSE)
            {
                if (RESPONSEBUFFER != null)
                {
                    Response.BinaryWrite(RESPONSEBUFFER);
                    Response.Flush();
                }
            }            
        }       
    }
}
