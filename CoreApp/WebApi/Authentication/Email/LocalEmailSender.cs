using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApi.Models.Settings;

namespace WebApi.Authentication.Email
{
    public class LocalEmailSender : IEmailSender
    {
        private readonly EmailSenderOptions _options;

        public LocalEmailSender(IOptions<EmailSenderOptions> options)
        {
            _options = options.Value;
        }
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            emailMessage.To.Add(new MailboxAddress(string.Empty, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart() { Text = htmlMessage };
            
            if (!Directory.Exists(_options.LocalDirectoryPath))
            {
                Directory.CreateDirectory(_options.LocalDirectoryPath);
            }
            
            using (var data = System.IO.File.CreateText($"{_options.LocalDirectoryPath}\\{Guid.NewGuid()}.html"))
            {
                emailMessage.WriteTo(data.BaseStream);
            }

            return Task.CompletedTask;
        }
    }
}