using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebApi.Models.Settings;

namespace WebApi.Authentication.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSenderOptions _options;

        public EmailSender(IOptions<EmailSenderOptions> options)
        {
            _options = options.Value;
        }
        
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            emailMessage.To.Add(new MailboxAddress(string.Empty, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                //todo: change secure options
                await client
                    .ConnectAsync(_options.SmtpServer, _options.SmtpPort, SecureSocketOptions.None)
                    .ConfigureAwait(false);

                //await client
                //    .AuthenticateAsync(_options.SmtpUsername, _options.SmtpPassword)
                //    .ConfigureAwait(false);

                await client
                    .SendAsync(emailMessage)
                    .ConfigureAwait(false);
                await client
                    .DisconnectAsync(true)
                    .ConfigureAwait(false);
            }

        }
    }
}