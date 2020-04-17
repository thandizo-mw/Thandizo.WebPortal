using System;
using System.Net.Http;
using System.Threading.Tasks;
using Thandizo.WebPortal.Helpers;

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
        public async Task<HttpResponseMessage> Post(string url, object value)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("Post"),
                RequestUri = new Uri(url),
                Content = new JsonContent(value)
            };
            request.Headers.Accept.Clear();
            return await client.SendAsync(request);
        }
        public async Task<HttpResponseMessage> Put(string url, object value)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("Put"),
                RequestUri = new Uri(url),
                Content = new JsonContent(value)
            };
            request.Headers.Accept.Clear();
            return await client.SendAsync(request);
        }
        public async Task<HttpResponseMessage> Delete(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("Delete"),
                RequestUri = new Uri(url),
            };
            request.Headers.Accept.Clear();
            return await client.SendAsync(request);
        }
    }
}
