using System.Net.Http;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Services
{
    public interface IHttpRequestHandler
    {
        Task<HttpResponseMessage> Get(string url);
        Task<HttpResponseMessage> Post(string url, object value);
        Task<HttpResponseMessage> Put(string url, object value);
        Task<HttpResponseMessage> Delete(string url);
    }
}