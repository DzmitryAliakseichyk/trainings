using System.Linq;
using Microsoft.Extensions.Options;
using WebApi.Models.Settings;

namespace WebApi.Email
{
    public class EmailTemplateProvider : IEmailTemplateProvider
    {
        private readonly EmailTemplate[] _emailTemplates;

        public EmailTemplateProvider(IOptions<EmailTemplates> emailTemplates)
        {
            _emailTemplates = emailTemplates.Value.Templates;
        }

        public string GetSubject(string templateName)
        {
            var template = _emailTemplates.FirstOrDefault(x => x.Name.Equals(templateName));
            return template?.Subject;
        }

        public string GetEmailBody(string templateName, string[] messages = null)
        {
            var template = _emailTemplates.FirstOrDefault(x => x.Name.Equals(EmailTemplateNames.ConfirmEmail));
            if (template != null)
            {
                return messages == null ? template.HtmlBody : string.Format(template.HtmlBody, messages);
            }

            return null;
        }
    }
}