using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Infrastructure.Mail
{
    public static class MailSendHelper
    {
        private static SmtpClient smtp;
        private static string server = ConfigurationManager.AppSettings["mail.server"]?? "smtp.exmail.qq.com";
        private static string username = ConfigurationManager.AppSettings["mail.username"] ?? "uoko-et-notify@uoko.com";
        private static string password = ConfigurationManager.AppSettings["mail.password"] ?? "Uoko2016";
        static MailSendHelper()
        {
            //邮件服务器和端口 
            SmtpClient smtp = new SmtpClient(server, 25);
            smtp.UseDefaultCredentials = true;
            //指定发送方式 
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //指定登录名和密码 
            smtp.Credentials = new System.Net.NetworkCredential(username, password);
            //超时时间 
            smtp.Timeout = 10000;
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="toIds">接收人Id集合</param>
        /// <param name="ccIds">抄送人Id集合</param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string SendMail(List<int> toIds, List<int> ccIds, string subject, string body)

        {
            var toUsers = UserHelper.GetUserByIds(toIds);
            var toEmails = string.Join(",", toUsers.Select(t => t.Email));
            var ccUsers = UserHelper.GetUserByIds(ccIds);
            var ccEmails = string.Join(",", ccUsers.Select(t => t.Email));

            return SendMail(toEmails,ccEmails,subject,body,null);
        }
        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string SendMail(string to,string cc, string subject, string body, List<string> files)

        {

            try

            {

                //邮件发送类 
                using (MailMessage mail = new MailMessage())
                {
                    //是谁发送的邮件 
                    mail.From = new MailAddress(username, "上线系统通知", Encoding.UTF8);
                    //发送给谁 
                    mail.To.Add(to);
                    //抄送给谁 
                    if (!string.IsNullOrEmpty(cc))
                    {
                        mail.CC.Add(cc);  
                    }
                    //标题 
                    mail.Subject = subject;
                    //内容编码 
                    mail.SubjectEncoding = mail.BodyEncoding = Encoding.UTF8;
                    //发送优先级 
                    mail.Priority = MailPriority.High;
                    //邮件内容 
                    mail.Body = body;
                    //是否HTML形式发送 
                    mail.IsBodyHtml = true;

                    //附件 
                    if (files!=null&&files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            mail.Attachments.Add(new Attachment(file));
                        }

                    }
                    smtp.Send(mail);
                    return "Ok";
                }
                   

            }

            catch (System.Exception exp)
            {

                return exp.Message;

            }

        }
    }
}
