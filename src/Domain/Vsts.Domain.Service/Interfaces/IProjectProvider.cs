using System.Collections.Generic;
using System.Threading.Tasks;
using Vsts.Domain.Contract.Response;

namespace Vsts.Domain.Service.Interfaces
{
    public interface IProjectProvider
    {
        Task<List<Project>> GetProjectsAsync();
        Task<Project> GetProjectAsync(string NameOrId);
    }
}
