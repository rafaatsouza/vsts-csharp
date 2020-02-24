using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Vsts.Domain.Service.Interfaces;

namespace Vsts.Infra.Provider
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public HttpClient GetHttpClient(string personalToken)
        {
            var key = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalToken}"));

            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", key);

            return client;
        }
    }
}