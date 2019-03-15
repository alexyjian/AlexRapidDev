using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    /// 发收邮件SDK
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// 发送邮件方法
        /// </summary>
        /// <param name="email">发件人地址</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static async Task SendEmail(string email, string title, string content)
        {
            ///配置邮件账户密码
            var username = ConfigurationManager.AppSettings["EmailUserName"];
            var password = ConfigurationManager.AppSettings["EmailPWD"];

            SmtpClient smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["EmailSMTP"];
            smtp.Credentials = new NetworkCredential(username, password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mail = new MailMessage(username, email);
            mail.Subject = title;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Body = content;
            mail.BodyEncoding = Encoding.UTF8;
            await smtp.SendMailAsync(mail);
        }
    }
}