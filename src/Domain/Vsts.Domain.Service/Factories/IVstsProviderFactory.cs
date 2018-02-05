using Vsts.Domain.Service.Interfaces;

namespace Vsts.Domain.Service.Factories
{
    public interface IVstsProviderFactory
    {
        IWorkItemProvider CreateWorkItemProvider(string apiVersion, string account, string teamProject, string token);
        IProjectProvider CreateProjectProvider(string apiVersion, string account, string token);
    }
}
