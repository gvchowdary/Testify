using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace AppNameTestify.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailComponent
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="ToAddress">To address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static bool SendEmail(string ToAddress, string subject, string body)
        {
            bool isSuccess = false;
            MailMessage mail = new MailMessage();
            try
            {

                string networkUsername = ConfigurationManager.AppSettings["NetworkUserName"];
                string networkPassword = ConfigurationManager.AppSettings["NetworkPassword"];
                var smtpClient = new SmtpClient();
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Host = ConfigurationManager.AppSettings["SmtpHostName"];
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortNo"]);
                smtpClient.Credentials = new NetworkCredential(networkUsername, networkPassword);
                ////smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["networkUsername"], ConfigurationManager.AppSettings["networkPassword"]);
                smtpClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
                mail.From = new MailAddress(networkUsername);
                mail.To.Add(ToAddress);
                mail.Body = body;
                mail.Subject = subject;
                mail.BodyEncoding = Encoding.Default;
                mail.IsBodyHtml = true;
                smtpClient.Send(mail);
               
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;

        }
    }
}