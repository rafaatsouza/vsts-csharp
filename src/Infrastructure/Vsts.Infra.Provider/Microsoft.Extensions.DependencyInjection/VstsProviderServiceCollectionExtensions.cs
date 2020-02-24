using System;
using Vsts.Domain.Contract.Dto;
using Vsts.Domain.Service.Interfaces;
using Vsts.Infra.Provider;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class VstsProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddVstsProvider(this IServiceCollection services, 
            VstsConfigurationData configurationData)
        {
            if (configurationData == null)
            {
                throw new ArgumentNullException(nameof(configurationData));
            }

            services.AddSingleton(configurationData);
            services.AddSingleton<IHttpClientProvider, HttpClientProvider>();
            services.AddSingleton<IVstsProvider, VstsProvider>();

            return services;
        }
    }
}