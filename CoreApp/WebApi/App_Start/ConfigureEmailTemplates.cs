using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebApi.Models.Settings;

namespace WebApi
{
    internal class ConfigureEmailTemplates
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailTemplates>(options =>
            {
                var rootPath = configuration.GetSection("Resources:MessagesPath").Value;
                var templatesJsonPath = Path.Combine(rootPath, "templates.json");

                string rawJson;
                using (var r = new StreamReader(templatesJsonPath))
                {
                    rawJson = r.ReadToEnd();
                }

                var emailTemplates = JsonConvert.DeserializeObject<EmailTemplates>(rawJson);

                foreach (var template in emailTemplates.Templates)
                {
                    var templatePath = Path.Combine(rootPath, $"{template.Name}.html");
                    using (var r = new StreamReader(templatePath))
                    {
                        template.HtmlBody = System.Web.HttpUtility.HtmlEncode(r.ReadToEnd());
                    }
                }

                options.Templates = emailTemplates.Templates;
            });
        }
    }
}
