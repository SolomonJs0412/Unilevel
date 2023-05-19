using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Unilever.v1.Common
{
    public class MailerService
    {
        public void SendMail(string recipient, string subject, string body)
        {

            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("997e556f69badb", "f15b049431093b"),
                EnableSsl = true
            };

            MailMessage message = new MailMessage();
            message.From = new MailAddress("noreply@unilevel.com.vn");
            message.To.Add(new MailAddress(recipient));
            message.Subject = subject;
            message.Body = body;

            client.Send(message);
        }

    }
}