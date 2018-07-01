using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.UtilsCore
{
    public class EmailHelper
    {
        public static Task<bool> SendEmailAsync(string email, string body, bool ishtml) 
        {
            var msg = new MailMessage();
            msg.To.Add(email);  
            msg.From = new MailAddress("yixinin@outlook.com", "Zlab", Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = "Zlab Registry Code";//邮件标题   
            msg.SubjectEncoding = Encoding.UTF8;//邮件标题编码   
            msg.Body = body;//邮件内容   
            msg.BodyEncoding = Encoding.UTF8;//邮件内容编码   
            msg.IsBodyHtml = ishtml;//是否是HTML邮件   
            msg.Priority = MailPriority.High;//邮件优先级   
            var client = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential("yixinin@outlook.com", "wangvivi0328"),
                //上述写你的GMail邮箱和密码   
                Port = 587,//Gmail使用的端口   
                Host = "smtp.outlook.com",
                EnableSsl = true//经过ssl加密   
            };
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState); 
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        
    }
}
