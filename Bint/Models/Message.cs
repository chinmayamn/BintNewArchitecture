using Microsoft.Extensions.Logging;
using System;
namespace Bint.Models
{
    public class Message:IMessage
    {
        private readonly ILogger<Message> _logger;
        public Message(ILogger<Message> logger)
        {
            _logger = logger;
        }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string EmailMessageBody { get; set; }
        public string SmsMessageBody { get; set; }
        public string MobileNumber { get; set; }

        public void SendMessage(Message message)
        {
            try
            {
                SendEmail(message);
                SendSms(message);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
           
        }
        public void SendEmail(Message message)
        {
                  var mto = message.To;
                var mFrom = "support@bintekglobal.com";

                try
                {
                    //if (mto != "" && mFrom != "")
                    //{

                    //    MailMessage message = new MailMessage();
                    //    message.From = new MailAddress(mFrom);
                    //    message.To.Add(mto);
                    //    message.IsBodyHtml = true;

                    //    message.Subject = _message.Subject;
                    //    message.Body = _message.EmailMessageBody;
                    //    SmtpClient smtp = new SmtpClient("relay-hosting.secureserver.net", 25);
                        
                    //    smtp.Send(message);
                    //    message.Dispose();

                    //}

                    //else
                    //{
                      
                    //}
                _logger.LogError("Subject : "+ message.Subject+" From : "+ message.From + " To : "+ message.To+" Message : "+ message.EmailMessageBody);

                }
                catch (Exception e)
                {
                 _logger.LogError(e.ToString());
                }
        }

        public string SendSms(Message message)
        {
            try
            {
                var result = "";
                //string message = HttpUtility.UrlEncode(_message.SMSMessageBody);

                //using (var wb = new WebClient())
                //{
                //    byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                //    {
                //        {"apikey", "HSmxGKHOCC4-wJfRLr2vnPYhHv97HS7tsZbYpaOLq2"},
                //        {"numbers", _message.MobileNumber},
                //        {"message", message},
                //        {"sender", "TXTLCL"}
                //    });
                //    result = System.Text.Encoding.UTF8.GetString(response);

                //}
                _logger.LogError("Mobile number : " + message.MobileNumber + " Message : " + message.SmsMessageBody);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

            }

            return "";
        }
    }
}
