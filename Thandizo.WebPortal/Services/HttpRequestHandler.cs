using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Services
{
    public class HttpRequestHandler:IHttpRequestHandler
    {
        public async Task<HttpResponseMessage> Get(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("Get"),
                RequestUri = new Uri(url),
            };
            request.Headers.Accept.Clear();
            return await client.SendAsync(request);
        }
    }
}
