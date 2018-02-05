using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vsts.Domain.Contract.Response;
using Vsts.Domain.Contract.Serializer;
using Vsts.Domain.Service.Interfaces;

namespace Vsts.Infra.Provider
{
    public class ProjectProvider : IProjectProvider, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiVersion;
        private readonly string _account;
        private readonly string _token;
        private readonly string _apiBaseUrl;

        public ProjectProvider(string apiVersion, string account, string token)
        {
            _apiVersion = apiVersion;
            _account = account;
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

        public async Task<List<Project>> GetProjectsAsync()
        {
            string url = $"{_apiBaseUrl}_apis/projects?api-version={_apiVersion}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(json))
                {
                    throw new InvalidCastException("Invalid Response Content from Vsts API - Is Null or Empty");
                }

                return VstsJson<List<Project>>.Deserialize(JObject.Parse(json).SelectToken("value").ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Project> GetProjectAsync(string NameOrId)
        {
            string url = $"{_apiBaseUrl}_apis/projects/{NameOrId}?includeCapabilities=false&api-version={_apiVersion}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(json))
                {
                    throw new InvalidCastException("Invalid Response Content from Vsts API - Is Null or Empty");
                }

                return VstsJson<Project>.Deserialize(json);

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
    }
}
