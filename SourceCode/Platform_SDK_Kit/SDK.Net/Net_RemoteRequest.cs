using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Net_RemoteRequest
    {

        public CookieContainer active_Cookies;

        public class_Net_RemoteRequest(ref CookieContainer refActiveCookieContainer)
        {
            this.active_Cookies = refActiveCookieContainer;
        }

        public class_Net_RemoteRequest()
        {

        }

        public byte[] getRemoteRequestToByteWithCookieHeader(string input, string remoteurl, int requestTimeOut, int buffersize)
        {
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(input);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.CookieContainer = new CookieContainer();
                if(active_Cookies.Count>0)                
                    request.CookieContainer = active_Cookies;
                request.Timeout = 1000 * 20;
                request.Method = "post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();
                Stream responseStream = ((HttpWebResponse)request.GetResponse()).GetResponseStream();
                string cookieheader = request.CookieContainer.GetCookieHeader(new Uri(remoteurl));
                active_Cookies.SetCookies(new Uri(remoteurl), cookieheader);                
                foreach(Cookie activeCookieInRequest in request.CookieContainer.GetCookies(new Uri(remoteurl)))           
                    active_Cookies.Add(activeCookieInRequest);                
                byte[] buffer2 = null;
                BinaryReader reader = new BinaryReader(responseStream);
                buffer2 = reader.ReadBytes(buffersize);
                reader.Close();
                responseStream.Close();
                return buffer2;
            }
            catch
            {
                return null;
            }
        }

        public string getRemoteRequestToStringWithCookieHeader(string input, string remoteurl, int requestTimeOut, int buffersize )
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.CookieContainer = new CookieContainer();
                if (active_Cookies.Count > 0)
                    request.CookieContainer = active_Cookies;
                request.Timeout = 0x1b7740;
                request.Method = "post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();
                StreamReader responseStream = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8); 
                string cookieheader = request.CookieContainer.GetCookieHeader(new Uri(remoteurl));
                active_Cookies.SetCookies(new Uri(remoteurl), cookieheader);                
                    foreach (Cookie activeCookieInRequest in request.CookieContainer.GetCookies(new Uri(remoteurl)))
                        active_Cookies.Add(activeCookieInRequest); 
                string result = "";                
                result = responseStream.ReadToEnd();
                return result;
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        public List<Cookie> getRemoteServerCookieFillCookieContainerByGet(string remoteurl)
        {
            try
            {
                List<Cookie> result = new List<Cookie>();
                HttpWebResponse response = null;
                HttpWebRequest request = null;                           
                request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.Timeout = 1000 * 60 * 2;
                request.Method = "get";                
                Thread.Sleep(1000);                
                response = (HttpWebResponse)request.GetResponse();
                Uri newUri = new Uri(remoteurl);
                if (response != null && request != null && response.StatusCode == HttpStatusCode.OK && request.CookieContainer.GetCookies(newUri).Count > 0)
                {
                    List<string> tmpstrCookiesList = new List<string>();
                    foreach (Cookie activeCookie in request.CookieContainer.GetCookies(newUri))
                    {
                        tmpstrCookiesList.Add(activeCookie.Domain + ":" + activeCookie.Name + "=" + activeCookie.Value);
                        active_Cookies.Add(activeCookie);
                        result.Add(activeCookie);
                    }                   
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public bool getRemoteRequestByGet(string remoteurl,int timeout = 1000*5)
        {
            try
            {                
                HttpWebResponse response = null;
                HttpWebRequest request = null;
                request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.Timeout = timeout;
                request.Method = "get";
                response = (HttpWebResponse)request.GetResponse();
                return true;                
            }
            catch
            {
                return false;
            }
        }

        public List<Cookie> getRemoteServerCookieFillCookieContainer(string remoteurl, string input,int timeout = 1000 * 10)
        {
            try
            {
                List<Cookie> result = new List<Cookie>();
                HttpWebResponse response = null;
                HttpWebRequest request = null;
                Stream requestStream = null;
                byte[] bytes = Encoding.Default.GetBytes(input);
                request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.Timeout = timeout;
                request.Method = "post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Thread.Sleep(1000);
                requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                response = (HttpWebResponse)request.GetResponse();
                Uri newUri = new Uri(remoteurl);
                if (response != null && request != null && requestStream != null && response.StatusCode == HttpStatusCode.OK && request.CookieContainer.GetCookies(newUri).Count > 0)
                {
                    List<string> tmpstrCookiesList = new List<string>();                    
                    foreach (Cookie activeCookie in request.CookieContainer.GetCookies(newUri))
                    {
                        tmpstrCookiesList.Add(activeCookie.Domain + ":" + activeCookie.Name + "=" + activeCookie.Value);
                        active_Cookies.Add(activeCookie);
                        result.Add(activeCookie);
                    }
                    requestStream.Close();
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public byte[] getRemoteRequestToByte(string input, string remoteurl, int requestTimeOut, int buffersize, List<Cookie> activeCookies)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.CookieContainer = new CookieContainer();
                if (activeCookies != null && activeCookies.Count > 0)
                    foreach (Cookie activeCookie in activeCookies)
                        request.CookieContainer.Add(activeCookie);
                request.Timeout = 0x1b7740;
                request.Method = "post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();
                Stream responseStream = ((HttpWebResponse)request.GetResponse()).GetResponseStream();
                byte[] buffer2 = null;
                BinaryReader reader = new BinaryReader(responseStream);
                buffer2 = reader.ReadBytes(buffersize);
                reader.Close();
                responseStream.Close();
                return buffer2;
            }
            catch
            {
                return null;
            }
        }

        public byte[] getRemoteXMLRequestToByte(string input, string remoteurl, int requestTimeOut, int buffersize, List<Cookie> activeCookies)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.CookieContainer = new CookieContainer();
                if (activeCookies != null && activeCookies.Count > 0)
                    foreach (Cookie activeCookie in activeCookies)
                        request.CookieContainer.Add(activeCookie);
                request.Timeout = 0x1b7740;
                request.Method = "post";
                request.ContentType = "text/xml";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();
                Stream responseStream = ((HttpWebResponse)request.GetResponse()).GetResponseStream();
                byte[] buffer2 = null;
                BinaryReader reader = new BinaryReader(responseStream);
                buffer2 = reader.ReadBytes(buffersize);
                reader.Close();
                responseStream.Close();
                return buffer2;
            }
            catch
            {
                return null;
            }
        }

        public string getRemoteRequestToString(string input, string remoteurl, int requestTimeOut, int buffersize, List<Cookie> activeCookies)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.CookieContainer = new CookieContainer();
                if (activeCookies != null && activeCookies.Count > 0)
                    foreach (Cookie activeCookie in activeCookies)
                        request.CookieContainer.Add(activeCookie);
                request.Timeout = 0x1b7740;
                request.Method = "post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();                
                StreamReader responseStream = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8);                
                string result = "";
                result = responseStream.ReadToEnd();
                return result;
            }
            catch(Exception err)
            {
                return err.Message;
            }
        }

        public string getRemoteXMLRequestToString(string input, string remoteurl, int requestTimeOut, int buffersize, List<Cookie> activeCookies)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.CookieContainer = new CookieContainer();
                if (activeCookies != null && activeCookies.Count > 0)
                    foreach (Cookie activeCookie in activeCookies)
                        request.CookieContainer.Add(activeCookie);
                request.Timeout = 0x1b7740;
                request.Method = "post";
                request.ContentType = "text/xml";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();
                StreamReader responseStream = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8); 
                string result = "";
                result = responseStream.ReadToEnd();
                return result;
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        public string sendBinDataToRemoteServer(string remoteurl, byte[] data,int timeout = 1000 * 10)
        {
            try
            {                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteurl);
                request.CookieContainer = new CookieContainer();
                if (active_Cookies.Count > 0)
                    request.CookieContainer = active_Cookies;
                request.Timeout = 0x1b7740;
                request.Method = "post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Flush();
                requestStream.Close();
                StreamReader responseStream = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8);
                string cookieheader = request.CookieContainer.GetCookieHeader(new Uri(remoteurl));
                active_Cookies.SetCookies(new Uri(remoteurl), cookieheader);
                foreach (Cookie activeCookieInRequest in request.CookieContainer.GetCookies(new Uri(remoteurl)))
                    active_Cookies.Add(activeCookieInRequest);
                string result = "";
                result = responseStream.ReadToEnd();
                return result;
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        public List<Cookie> getRemoteServerCookie(string remoteurl, string input, int timeout = 1000 * 10)
        {
            try
            {
                List<Cookie> result = new List<Cookie>();
                HttpWebResponse response = null;
                HttpWebRequest request = null;
                Stream requestStream = null;
                byte[] bytes = Encoding.Default.GetBytes(input);
                request = (HttpWebRequest)WebRequest.Create(remoteurl);
                CookieContainer cookies = new CookieContainer();
                request.Timeout = timeout;
                request.Method = "post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                request.CookieContainer = cookies;
                Thread.Sleep(1000);
                requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                response = (HttpWebResponse)request.GetResponse();             
                Uri newUri = new Uri(remoteurl);
                if (response != null && request != null && requestStream != null && response.StatusCode == HttpStatusCode.OK && request.CookieContainer.GetCookies(newUri).Count > 0)
                {
                    List<string> tmpstrCookiesList = new List<string>();
                    foreach (Cookie activeCookie in request.CookieContainer.GetCookies(newUri))
                    {
                        tmpstrCookiesList.Add(activeCookie.Domain + ":" + activeCookie.Name + "=" + activeCookie.Value);
                        result.Add(activeCookie);
                    }
                    requestStream.Close();
                }              
                return result;
            }
            catch 
            {
                return null;
            }
        }

    }
}
