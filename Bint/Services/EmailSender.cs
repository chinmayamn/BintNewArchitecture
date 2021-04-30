using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bint.Models;
using Microsoft.Extensions.Logging;
namespace Bint.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<Message> _logger;

        public EmailSender(ILogger<Message> logger)
        {
            _logger = logger;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            Message m = new Message(_logger)
            {
                EmailMessageBody = message,
                To = email,
                Subject = subject
            };
            m.SendEmail(m);
            return Task.CompletedTask;
        }
    }
}
