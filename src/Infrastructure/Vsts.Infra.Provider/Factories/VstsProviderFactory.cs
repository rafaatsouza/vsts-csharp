using System;
using Vsts.Domain.Service.Factories;
using Vsts.Domain.Service.Interfaces;

namespace Vsts.Infra.Provider.Factories
{
    public class VstsProviderFactory : IVstsProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        
        public VstsProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IVstsProvider Create(string apiVersion, string account, string teamProject, string token)
        {
            return new VstsProvider(apiVersion, account, teamProject, token);
        }
    }
}
