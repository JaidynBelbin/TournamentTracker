using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TrackerLibrary
{
    public static class EmailLogic
    {
        public static void SendEmail(string to, string subject, string body)
        {
            SendEmail(new List<string> { to }, new List<string>(), subject, body);
        }

        /// <summary>
        /// Overloaded method in the case of multiple recipients
        /// </summary>
        /// <param name="to"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendEmail(List<string> to, List<string> bcc, string subject, string body)
        {

            MailAddress senderAddress = new MailAddress(GlobalConfig.AppKeyLookup("senderEmail"), GlobalConfig.AppKeyLookup("senderDisplayName"));

            MailMessage email = new MailMessage();

            foreach (string recipient in to)
            {
                email.To.Add(recipient); 
            }

            foreach (string recipient in bcc)
            {
                email.Bcc.Add(recipient);
            }

            email.From = senderAddress;
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();

            client.Send(email);
        }
    }
}
