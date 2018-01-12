using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vsts.Domain.Contract.Response;

namespace Vsts.Domain.Service.Interfaces
{
    public interface IVstsProvider
    {
        Task<int> CreateWorkItemAsync(string workItemType, string subject, string description, List<Tuple<string, string>> NameValueFieldsList);

        Task<List<WorkItem>> GetWorkItemAsync(List<int> workItemIds, List<string> fields);

        Task<WorkItemQueryResponse> GetWorkItemIdAsync(string where = null);

        Task<List<string>> GetWorkItemColumnsAsync();
    }
}
