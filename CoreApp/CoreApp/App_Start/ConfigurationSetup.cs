using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CoreApp.App_Start
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
