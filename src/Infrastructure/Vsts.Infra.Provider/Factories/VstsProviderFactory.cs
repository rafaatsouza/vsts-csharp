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

        public IWorkItemProvider CreateWorkItemProvider(string apiVersion, string account, string teamProject, string token)
        {
            return new WorkItemProvider(apiVersion, account, teamProject, token);
        }

        public IProjectProvider CreateProjectProvider(string apiVersion, string account, string token)
        {
            return new ProjectProvider(apiVersion, account, token);
        }
    }
}
