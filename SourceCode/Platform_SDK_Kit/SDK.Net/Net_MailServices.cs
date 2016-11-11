using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Net_MailServices
    {
        public bool SendMail(string server, string account, string password, List<string> mailto, string header, string content)
        {
            try
            {                

                string f_mailto=mailto[0].TrimStart();
                f_mailto=mailto[0].TrimEnd();
                MailAddress to = new MailAddress(f_mailto);
                account=account.TrimStart();
                account=account.TrimEnd();
                MailAddress from = new MailAddress(account);
                MailMessage message = new MailMessage(from, to);
                if(mailto.Count>1)
                {
                    for(int i=1;i<mailto.Count;i++)
                    {
                        string result="";
                        result= mailto[i].TrimStart();
                        result=mailto[i].TrimEnd();
                        message.To.Add(result);
                    }                    
                }              
                message.Subject = header;
                message.IsBodyHtml = true;
                message.Body = content;
                new SmtpClient(server) { Credentials = new NetworkCredential(account, password) }.Send(message);
                return true;
            }
            catch 
            {
                return false;
            }
        }      
    }
}
