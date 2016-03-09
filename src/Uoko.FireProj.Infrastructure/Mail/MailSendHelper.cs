using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Mail;
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
            smtp = new SmtpClient(server, 25);
            smtp.UseDefaultCredentials = true;
            //指定发送方式 
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //指定登录名和密码 
            smtp.Credentials = new System.Net.NetworkCredential(username, password);
            //超时时间 
            smtp.Timeout = 10000;
        }
        /// <summary>
        /// 发送测试结果通知
        /// </summary>
        /// <param name="toIds"></param>
        /// <param name="ccIds"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public static string NotifyTestResult(List<int> toIds, List<int> ccIds, QANotifyMail notify)
        {
            var subject = string.Format("【{0}】  在 {1} 中测试任务【{2}】 {3}", notify.TestUser, notify.StageName,notify.TaskName,notify.TestResult);
            #region body内容组装
            StringBuilder content = new StringBuilder();
            content.AppendFormat("<h4>【{0}】  在 {1} 中测试任务【{2}】 {3}</h4>", notify.TestUser, notify.StageName, notify.TaskName, notify.TestResult);
            content.AppendFormat("<p>描述：{0}</p>", notify.Coments);
            if (notify.IsAllPassed)
            {
                content.AppendFormat("<p style='color:red;font-size:16px;'>当前环境测试全部通过</p>");
            }
            content.AppendFormat("测试地址：<a href='{0}'>{1}</a>", notify.TestUrl, notify.TestUrl);
            content.AppendFormat("<a href='{0}'>任务详细</a>", notify.TaskUrl);
            #endregion

            var body = content.ToString();
            return SendMail(toIds, ccIds, subject, body);
        }

        /// <summary>
        /// 发送部署结果通知
        /// </summary>
        /// <param name="toIds"></param>
        /// <param name="ccIds"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public static string NotifyDeployResult(List<int> toIds, List<int> ccIds, DeployNotifyMail notify)
        {
            var subject = string.Format("[{0}] 在 {1} 中 {2}", notify.TaskName, notify.StageName, notify.DeployStatus);

            #region body内容组装
            StringBuilder content = new StringBuilder();
            content.AppendFormat("<h4>[{0}] 在 {1} 中 {2}</h4>", notify.TaskName, notify.StageName, notify.DeployStatus);
            if (!string.IsNullOrEmpty(notify.DeployUrl))
            {
                content.AppendFormat("测试地址：<a href='{0}'>{1}</a>", notify.DeployUrl, notify.DeployUrl);
            }  
            if (!string.IsNullOrEmpty(notify.GitLabBuildPage))
            {
               content.AppendFormat("部署详细：<a href='{0}'>{1}</a>", notify.GitLabBuildPage, notify.GitLabBuildPage); 
            }        
            content.AppendFormat("<a href='{0}'>任务详细</a>", notify.TaskUrl);
            #endregion

            var body = content.ToString();
            return SendMail(toIds, ccIds, subject, body);
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
            #region 去掉重复
            toIds = toIds.Distinct().ToList();
            ccIds = ccIds.Where(t => !toIds.Contains(t)).ToList(); 
            #endregion
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
                    mail.Priority = MailPriority.Normal;
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
