using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vsts.Domain.Contract;
using Vsts.Domain.Contract.Dto;
using Vsts.Domain.Contract.Response;
using Vsts.Domain.Contract.Serializer;
using Vsts.Domain.Service.Interfaces;

namespace Vsts.Infra.Provider
{
    public class VstsProvider : IVstsProvider
    {
        private readonly IHttpClientProvider httpClientProvider;
        private readonly string teamProject;
        private readonly string token;
        private readonly string apiBaseUrl;

        private const string ApiVersion = "5.1";

        public VstsProvider(IHttpClientProvider httpClientProvider,
            VstsConfigurationData configurationData)
        {
            if (configurationData == null)
            {
                throw new ArgumentNullException(nameof(configurationData));
            }

            this.httpClientProvider = httpClientProvider
                ?? throw new ArgumentNullException(nameof(httpClientProvider));
            this.token = configurationData.Token
                ?? throw new ArgumentException($"{nameof(configurationData.Token)} null or empty");
            this.teamProject = configurationData.TeamProject
                ?? throw new ArgumentException($"{nameof(configurationData.TeamProject)} null or empty");

            if (string.IsNullOrEmpty(configurationData.Organization))
            {
                throw new ArgumentException($"{nameof(configurationData.Organization)} null or empty");
            }

            apiBaseUrl = $"https://{configurationData.Organization}.visualstudio.com/DefaultCollection/";
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            var reg = new Regex(@"(?<![\w\d])skip(?![\w\d])(\=(\d+))");

            var url = $"{apiBaseUrl}_apis/projects?api-version={ApiVersion}&$skip=0";
            var completed = false;
            var projects = new List<Project>();

            while (!completed)
            {
                if (projects.Any())
                {
                    url = reg.Replace(url, $"skip={projects.Count.ToString()}");
                }

                using (var httpClient = httpClientProvider.GetHttpClient(token))
                using (var response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    if (int.Parse(JObject.Parse(json).SelectToken("count").ToString()) == 0)
                    {
                        completed = true;
                    }
                    else
                    {
                        projects.AddRange(VstsJson<IEnumerable<Project>>
                            .Deserialize(JObject.Parse(json).SelectToken("value").ToString()));
                    }
                }
            }

            return projects;
        }

        public async Task<Project> GetProjectAsync(string NameOrId)
        {
            var url = $"{apiBaseUrl}_apis/projects/{NameOrId}?includeCapabilities=false&api-version={ApiVersion}";

            using (var httpClient = httpClientProvider.GetHttpClient(token))
            using (var response = await httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                return VstsJson<Project>.Deserialize(json);
            }
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsAsync(string where)
        {
            var url = $"{apiBaseUrl}{teamProject}/_apis/wit/wiql?api-version={ApiVersion}";
            var clause = where != null ? "WHERE " + where : "";

            var result = new List<WorkItem>();

            var workItemQueryRequestJson = VstsJson<WorkItemQueryRequest>
                .Serialize(new WorkItemQueryRequest { Query = $"SELECT * FROM WorkItems {clause}" });
            var content = new StringContent(workItemQueryRequestJson, Encoding.UTF8, "application/json");

            using (var httpClient = httpClientProvider.GetHttpClient(token))
            using (var response = await httpClient.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var workItemsResponse = VstsJson<WorkItemQueryResponse>.Deserialize(json);

                foreach (var workItem in workItemsResponse.WorkItems)
                {
                    using (var workItemResponse = await httpClient.GetAsync(workItem.Url))
                    {
                        var workItemJson = await workItemResponse.Content.ReadAsStringAsync();

                        result.Add(VstsJson<WorkItem>.Deserialize(workItemJson));
                    }
                }
            }

            return result;
        }

        public async Task<int> CreateWorkItemAsync(string workItemType, string subject, string description, List<Tuple<string, string>> NameValueFieldsList)
        {
            NameValueFieldsList.Add(new Tuple<string, string>("System.WorkItemType", workItemType));
            NameValueFieldsList.Add(new Tuple<string, string>("System.Title", subject));
            NameValueFieldsList.Add(new Tuple<string, string>("System.Description", description));

            var FieldList = new List<WorkItemFieldsNameValuePair>();

            foreach (var item in NameValueFieldsList)
            {
                FieldList.Add(new WorkItemFieldsNameValuePair()
                {
                    op = "add",
                    path = $"/fields/{item.Item1}",
                    value = item.Item2
                });
            }

            var FieldValuesRequestJson = VstsJson<List<WorkItemFieldsNameValuePair>>.Serialize(FieldList);
            var content = new StringContent(FieldValuesRequestJson, Encoding.ASCII, "application/json-patch+json");

            string url = $"{apiBaseUrl}{teamProject}/_apis/wit/workitems/$Task?api-version={ApiVersion}";

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };

            using (var httpClient = httpClientProvider.GetHttpClient(token))
            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(json))
                {
                    throw new InvalidCastException("Invalid Response Content from Vsts API - Is Null or Empty");
                }

                return int.Parse(JObject.Parse(json).SelectToken("id").ToString());
            }
        }
    }
}