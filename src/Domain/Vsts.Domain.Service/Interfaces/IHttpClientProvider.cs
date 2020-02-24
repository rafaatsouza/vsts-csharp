using System.Net.Http;

namespace Vsts.Domain.Service.Interfaces
{
    public interface IHttpClientProvider
    {
        HttpClient GetHttpClient(string personalToken);
    }
}