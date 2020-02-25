using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vsts.Domain.Contract.Dto;
using Vsts.Domain.Contract.Response;

namespace Vsts.Domain.Service.Interfaces
{
    public interface IVstsProvider
    {
        /// <summary>
        /// Creates work item and returns its ID
        /// </summary>
        /// <param name="workItemType"></param>
        /// <param name="subject"></param>
        /// <param name="description"></param>
        /// <param name="nameValueFieldsList"></param>
        /// <returns>Work item ID</returns>
        Task<int> CreateWorkItemAsync(string workItemType, string subject, string description, List<Tuple<string, string>> nameValueFieldsList);

        /// <summary>
        /// Returns work item list
        /// </summary>
        /// <param name="where"></param>
        /// <returns>List of work items <see cref="IEnumerable{WorkItem}"/></returns>
        Task<IEnumerable<WorkItem>> GetWorkItemsAsync(string where);

        /// <summary>
        /// Returns project list
        /// </summary>
        /// <returns>List of work items <see cref="IEnumerable{Project}"/></returns>
        Task<IEnumerable<Project>> GetProjectsAsync();

        /// <summary>
        /// Returns project by its name or id
        /// </summary>
        /// <param name="NameOrId"></param>
        /// <returns>Project <see cref="Project"/></returns>
        Task<Project> GetProjectAsync(string NameOrId);
    }
}