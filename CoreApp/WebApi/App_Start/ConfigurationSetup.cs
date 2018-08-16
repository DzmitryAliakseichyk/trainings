using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    internal class ConfigurationSetup
    {
        internal static void Configure(WebHostBuilderContext ctx, IConfigurationBuilder configuration)
        {
            configuration
                .SetBasePath(ctx.HostingEnvironment.ContentRootPath);
        }
    }
}
