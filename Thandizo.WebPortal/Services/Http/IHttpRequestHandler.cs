using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Services
{
    public interface IHttpRequestHandler
    {
        Task<HttpResponseMessage> Delete(string requestUri);
        Task<HttpResponseMessage> Delete(string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Delete(string accessToken, string requestUri);
        Task<HttpResponseMessage> Delete(string accessToken, string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Delete(string username, string password, string requestUri);
        Task<HttpResponseMessage> Delete(string username, string password, string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Get(string requestUri);
        Task<HttpResponseMessage> Get(string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Get(string accessToken, string requestUri);
        Task<HttpResponseMessage> Get(string accessToken, string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Get(string username, string password, string requestUri);
        Task<HttpResponseMessage> Get(string username, string password, string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Patch(string requestUri, object value);
        Task<HttpResponseMessage> Patch(string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Patch(string accessToken, string requestUri, object value);
        Task<HttpResponseMessage> Patch(string accessToken, string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Post(string requestUri, object value);
        Task<HttpResponseMessage> Post(string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Post(string accessToken, string requestUri, object value);
        Task<HttpResponseMessage> Post(string accessToken, string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Post(string username, string password, string requestUri, object value);
        Task<HttpResponseMessage> Post(string username, string password, string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Put(string requestUri, object value);
        Task<HttpResponseMessage> Put(string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Put(string accessToken, string requestUri, object value);
        Task<HttpResponseMessage> Put(string accessToken, string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);
        Task<HttpResponseMessage> Put(string username, string password, string requestUri, object value);
        Task<HttpResponseMessage> Put(string username, string password, string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields);

    }
}