﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vsts.Domain.Contract;
using Vsts.Domain.Contract.Response;
using Vsts.Domain.Contract.Serializer;
using Vsts.Domain.Service.Interfaces;

namespace Vsts.Infra.Provider
{
    public class VstsProvider : IVstsProvider, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiVersion;
        private readonly string _account;
        private readonly string _teamProject;
        private readonly string _token;
        private readonly string _apiBaseUrl;

        public VstsProvider(string apiVersion, string account, string teamProject, string token)
        {
            _apiVersion = apiVersion;
            _account = account;
            _teamProject = teamProject;
            _token = token;
            _apiBaseUrl = $"https://{_account}.visualstudio.com/DefaultCollection/";
            _httpClient = GetHttpClient();
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _token))));

            return client;
        }

        public async Task<WorkItemQueryResponse> GetWorkItemIdAsync(string where)
        {
            string url = $"{_apiBaseUrl}{_teamProject}/_apis/wit/wiql?api-version={_apiVersion}";

            string clause = where != null ? "WHERE " + where : "";

            var workItemQueryRequestJson = VstsJson<WorkItemQueryRequest>.Serialize(new WorkItemQueryRequest { Query = $"SELECT * FROM WorkItems {clause}" });

            var content = new StringContent(workItemQueryRequestJson, Encoding.ASCII, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(json))
                {
                    throw new InvalidCastException("Invalid Response Content from Vsts API - Is Null or Empty");
                }

                return VstsJson<WorkItemQueryResponse>.Deserialize(json);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<WorkItem>> GetWorkItemAsync(List<int> workItemIds, List<string> fields)
        {
            string url = $"{_apiBaseUrl}_apis/wit/WorkItems?ids={string.Join(",", workItemIds)}&fields={string.Join(",", fields)}&api-version={_apiVersion}";

            using (HttpResponseMessage response = await _httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    throw new InvalidCastException("Invalid Response Content from Vsts API - Is Null or Empty");
                }

                return VstsJson<List<WorkItem>>.Deserialize((JObject.Parse(content).SelectToken("value")).ToString());
            }
        }

        public async Task<List<string>> GetWorkItemColumnsAsync()
        {
            string url = $"{_apiBaseUrl}{_teamProject}/_apis/wit/fields?api-version={_apiVersion}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(json))
                {
                    throw new InvalidCastException("Invalid Response Content from Vsts API - Is Null or Empty");
                }

                return (JObject.Parse(json)).SelectToken("value").Select(x => (string)x.SelectToken("referenceName")).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
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
                        
            string url = $"{_apiBaseUrl}{_teamProject}/_apis/wit/workitems/$Task?api-version={_apiVersion}";
            
            try
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = content
                };

                using (var response = await _httpClient.SendAsync(request))
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
