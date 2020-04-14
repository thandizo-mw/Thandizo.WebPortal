using System.Net.Http;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Services
{
    public interface IHttpRequestHandler
    {
        Task<HttpResponseMessage> Get(string url);
    }
}