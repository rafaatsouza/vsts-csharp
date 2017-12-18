using System.Collections.Generic;
using System.Threading.Tasks;
using Vsts.Domain.Contract.Response;

namespace Vsts.Domain.Service.Interfaces
{
    public interface IVstsProvider
    {
        Task<List<WorkItem>> GetWorkItemAsync(int[] workItemIds, string[] fields);

        Task<WorkItemQueryResponse> GetWorkItemIdAsync(string where = null);
    }
}
