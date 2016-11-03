using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace iKCoder_Platform_SDK_Kit
{

    internal class Base_EmailEntry
    {
        public string mailto;
        public string subject;
        public string content;
    }

    public class Base_EmailServices
    {
        Base_EmailIntent ActiveEmailIntent;
        Thread ServicesThread;

        Queue<Base_EmailEntry> MailTaskQueue = new Queue<Base_EmailEntry>();        

        public Base_EmailServices(string m_smtp, string m_from, string m_fromPWD)
        {
            ActiveEmailIntent = new Base_EmailIntent(m_smtp, m_from, m_fromPWD);
        }

        public bool Start_Services()
        {
            try
            {
                ServicesThread = new Thread(new ThreadStart(Services_Monitor));
                ServicesThread.IsBackground = true;
                ServicesThread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop_Services()
        {
            try
            {
                ServicesThread.Abort();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Clear_AllTasks()
        {
            try
            {
                MailTaskQueue.Clear();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Services_Monitor()
        {
            while (true)
            {
                Base_EmailEntry activeEntry = MailTaskQueue.Dequeue();
                if (activeEntry != null)
                {
                    ActiveEmailIntent.SendMail(activeEntry.mailto, activeEntry.subject, activeEntry.content);
                    continue;
                }
                Thread.Sleep(1000);
            }
        }

        public void SetMailTask(string mailto, string subject, string content)
        {
            Base_EmailEntry newEntry = new Base_EmailEntry();
            newEntry.mailto = mailto;
            newEntry.subject = subject;
            newEntry.content = content;
            MailTaskQueue.Enqueue(newEntry);                

        }       

    }

    public class Base_EmailIntent
    {
        public string m_from;
        public string m_fromPWD;
        public string m_smtp;

        private SmtpClient activeSmtpClientObj;
        private MailAddress addFrom;

        public Base_EmailIntent(string m_smtp, string m_from, string m_fromPWD)
        {
            this.m_from = m_from;
            this.m_smtp = m_smtp;
            activeSmtpClientObj = new SmtpClient(m_smtp);
            activeSmtpClientObj.Credentials = new NetworkCredential(this.m_from, this.m_fromPWD);
            addFrom = new MailAddress(this.m_from);
        }

        public bool SendMail(string mailto, string mailsubject, string mailcontent)
        {
            try
            {
                MailAddress addTo = new MailAddress(mailto);
                MailMessage messTo = new MailMessage(addFrom, addTo);
                messTo.Body = mailcontent;
                messTo.IsBodyHtml = true;
                messTo.Subject = mailsubject;
                activeSmtpClientObj.Send(messTo);
                return true;
            }
            catch
            {
                return false;
            }
        }
      
    }

}
