namespace WebApi.Email
{
    public interface IEmailTemplateProvider
    {
        string GetSubject(string templateName);
        string GetEmailBody(string templateName, string[] messages = null);
    }
}