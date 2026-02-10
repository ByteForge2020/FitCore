using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Api.BaseConfiguration
{
    public static class BaseWebApplicationBuilderExtensions
    {
        public static void AddBasicMicroserviceFeatures(this WebApplicationBuilder applicationBuilder)
        {
            // Configuration
            applicationBuilder.Services.AddSingleton<IConfiguration>(applicationBuilder.Configuration);

            // Logging
            applicationBuilder.Host.UseLogging();
            
            // TODO: Health Checks
        }
    }
}
