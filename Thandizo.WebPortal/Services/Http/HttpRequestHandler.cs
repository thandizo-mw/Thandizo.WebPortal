using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Thandizo.WebPortal.Helpers;

namespace Thandizo.WebPortal.Services
{
    public class HttpRequestHandler : IHttpRequestHandler
    {
        public HttpRequestHandler()
        {
        }

        //public async Task<HttpResponseMessage> Get(string url)
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage
        //    {
        //        Method = new HttpMethod("Get"),
        //        RequestUri = new Uri(url),
        //    };
        //    request.Headers.Accept.Clear();
        //    return await client.SendAsync(request);
        //}
        //public async Task<HttpResponseMessage> Post(string url, object value)
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage
        //    {
        //        Method = new HttpMethod("Post"),
        //        RequestUri = new Uri(url),
        //        Content = new JsonContent(value)
        //    };
        //    request.Headers.Accept.Clear();
        //    return await client.SendAsync(request);
        //}
        //public async Task<HttpResponseMessage> Put(string url, object value)
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage
        //    {
        //        Method = new HttpMethod("Put"),
        //        RequestUri = new Uri(url),
        //        Content = new JsonContent(value)
        //    };
        //    request.Headers.Accept.Clear();
        //    return await client.SendAsync(request);
        //}
        //public async Task<HttpResponseMessage> Delete(string url)
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage
        //    {
        //        Method = new HttpMethod("Delete"),
        //        RequestUri = new Uri(url),
        //    };
        //    request.Headers.Accept.Clear();
        //    return await client.SendAsync(request);
        //}

        /// <summary>
        /// This method builds and send GET HTTP request
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get(string requestUri)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send GET HTTP request with custom header fields
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get(string requestUri,
            IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send GET HTTP request with authentication header information
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get(string username, string password,
            string requestUri)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddAuthenticationHeader(username, password);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send GET HTTP request with authentication header and custom header fields
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get(string username, string password,
            string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddAuthenticationHeader(username, password)
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send GET HTTP request with authorization token information
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <param name="requestUri">The request URI</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get(string accessToken, string requestUri)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(accessToken);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send GET HTTP request with authorization token and custom header fields
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="requestUri"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get(string accessToken, string requestUri,
            IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(accessToken)
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send POST HTTP request
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post(string requestUri, object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value));

            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send POST HTTP request with custom header fields
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post(string requestUri, object value,
            IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddCustomHeaderFields(customHeaderFields);

            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send POST HTTP request with authentication header information
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post(string username, string password,
           string requestUri, object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Post)
                                .AddAuthenticationHeader(username, password)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value));

            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send POST HTTP request with authentication header and custom header fields
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post(string username, string password,
           string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Post)
                                .AddAuthenticationHeader(username, password)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddCustomHeaderFields(customHeaderFields);

            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send POST HTTP request with bearer access tokens information
        /// </summary>
        /// <param name="accessToken">The authorization token</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post(string accessToken, string requestUri,
            object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                .AddMethod(HttpMethod.Post)
                .AddBearerToken(accessToken)
                .AddRequestUri(requestUri)
                .AddContent(new JsonContent(value));
            return await builder.SendAsync();
        }

        public async Task<HttpResponseMessage> Post(string accessToken, string requestUri,
            object value, IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                .AddMethod(HttpMethod.Post)
                .AddBearerToken(accessToken)
                .AddRequestUri(requestUri)
                .AddContent(new JsonContent(value))
                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PUT HTTP request with authorization token information
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="value">The value data</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Put(string accessToken, string requestUri, object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(accessToken)
                                .AddContent(new JsonContent(value));
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PUT HTTP request with authorization token and custom header fields
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="value">The value data</param>
        /// <param name="customHeaderFields">Custom header fields list</param>
        public async Task<HttpResponseMessage> Put(string accessToken, string requestUri, object value,
            IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(accessToken)
                                .AddContent(new JsonContent(value))
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PUT HTTP request with authentication header information
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Put(string username, string password,
           string requestUri, object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddAuthenticationHeader(username, password)
                                .AddContent(new JsonContent(value));

            return await builder.SendAsync();
        }

        /// <summary>
        ///  This method builds and send PUT HTTP request with authentication header information and custom header fields
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Put(string username, string password,
           string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddAuthenticationHeader(username, password)
                                .AddContent(new JsonContent(value))
                                .AddCustomHeaderFields(customHeaderFields);

            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PUT HTTP request
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Put(
           string requestUri, object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value));
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PUT HTTP request with custom header fields
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Put(
           string requestUri, object value, IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PATCH HTTP request with authentication header information
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Patch(string requestUri, object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddContent(new PatchContent(value));
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PATCH HTTP request with authentication header and custom header fields
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Patch(string requestUri, object value,
            IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddContent(new PatchContent(value))
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PATCH HTTP request with authentication header information
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Patch(string accessToken, string requestUri,
            object value)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddBearerToken(accessToken)
                                .AddContent(new PatchContent(value));
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send PATCH HTTP request with authentication header and custom header fields
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Patch(string accessToken, string requestUri,
           object value, IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddBearerToken(accessToken)
                                .AddContent(new PatchContent(value))
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send DELETE HTTP request
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(string requestUri)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Delete)
                                .AddRequestUri(requestUri);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send DELETE HTTP request with custom header fields
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(string requestUri,
            IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Delete)
                                .AddRequestUri(requestUri)
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and sends DELETE HTTP request with an authorization token
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <param name="requestUri">the request URI</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(string accessToken, string requestUri)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Delete)
                                .AddBearerToken(accessToken)
                                .AddRequestUri(requestUri);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and sends DELETE HTTP request with an authorization token and custom header fields
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="requestUri"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(string accessToken, string requestUri,
            IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Delete)
                                .AddBearerToken(accessToken)
                                .AddRequestUri(requestUri)
                                .AddCustomHeaderFields(customHeaderFields);
            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send DELETE HTTP request with authentication header information
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(string username, string password,
            string requestUri)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Delete)
                                .AddAuthenticationHeader(username, password)
                                .AddRequestUri(requestUri);

            return await builder.SendAsync();
        }

        /// <summary>
        /// This method builds and send DELETE HTTP request with authentication header and custom header fields
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="requestUri"></param>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(string username, string password,
            string requestUri, IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            var builder = new HttpRequestBuilder(PolicyFactory.CreatePolicies())
                                .AddMethod(HttpMethod.Delete)
                                .AddAuthenticationHeader(username, password)
                                .AddRequestUri(requestUri)
                                .AddCustomHeaderFields(customHeaderFields);

            return await builder.SendAsync();
        }
    }
}
