using System.Net.Mail;
//using Microsoft.Office.Interop.Outlook;

namespace rehome.Public
{
    public static class MailClass
    {
        public static void SendMail(string toAddress, string subject, string body)
        {
            // メールオブジェクトを作成
            
            MailMessage mailMessage = new MailMessage();
  //         Application outlook = new Application();

            // 送信元アドレスを設定
            mailMessage.From = new MailAddress("murai.m@hbm-web.co.jp");

            // 送信先アドレスを設定
            mailMessage.To.Add(toAddress);

            // 件名を設定
            mailMessage.Subject = subject;

            // 本文を設定
            mailMessage.Body = body;

            // SMTPを使用してメールを送信
            using (SmtpClient smtpClient = new SmtpClient())
            {
                
                smtpClient.Host = "murai.m@hbm-web.co.jp";
                smtpClient.Port = 25;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);
            }
        }
    }
}
