using Vsts.Domain.Service.Interfaces;

namespace Vsts.Domain.Service.Factories
{
    public interface IVstsProviderFactory
    {
        IVstsProvider Create(string apiVersion, string account, string teamProject, string token);
    }
}
