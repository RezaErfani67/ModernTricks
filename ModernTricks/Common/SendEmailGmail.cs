using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SendMail
{
    public class SendEmailGmail
    {
        public static void Send(string To,string Subject,string Body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("erfani.mr@aharco.com","توسعه دهنده برنامه های تحت وب");
            mail.To.Add(To);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            mail.Headers.Add("Disposition-Notification-To", "sender@mainhost.com");
            mail.ReplyTo= new MailAddress("erfani.mr@aharco.com", "شرکت آسیا طوس شرق") ;
            //System.Net.Mail.Attachment attachment;
            // attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
            // mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("rezaerfani67@gmail.com", "@861140311");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);

        }
    }
}