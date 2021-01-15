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

            MailAddress senderAddress = new MailAddress(GlobalConfig.AppKeyLookup("senderEmail"), GlobalConfig.AppKeyLookup("senderDisplayName"));

            MailMessage email = new MailMessage();

            email.To.Add(to);
            email.From = senderAddress;
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();

            client.Send(email);
        }
    }
}
