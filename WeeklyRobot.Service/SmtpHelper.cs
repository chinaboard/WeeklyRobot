using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace WeeklyRobot.Service
{
    public static class SmtpHelper
    {
        public static void SendMail(string title, string content)
        {
            Logger.Debug($"SendMail\t{title}");
            var client = new SmtpClient();
            client.Host = Config.TryGet("SMTPServer", "smtp.163.com");
            client.Port = Config.TryGet("SMTPPort", 25);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(Config.TryGet("SMTPUser", "user"), Config.TryGet("SMTPPwd", "pwd"));

            //电子邮件信息类
            var fromAddress = new MailAddress(Config.TryGet("FromAddress", "user"), "WeeklyRobot");
            var toAddress = new MailAddress(Config.Get<string>("ToAddress"));
            var mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = title;

            mailMessage.Body = content;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.High;
            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
    }
}
