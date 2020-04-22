using Polly;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace Thandizo.WebPortal.Services
{

    public class HttpRequestBuilder
    {
        private HttpMethod method = null;
        private HttpClient _client;
        private string requestUri = "";
        private HttpContent content = null;
        private string username = "";
        private string password = "";
        private string bearerToken = "";
        private string acceptHeader = "application/json";
        private TimeSpan timeout = TimeSpan.FromSeconds(15);
        private PolicyWrap _policyWrapper;
        private IEnumerable<HttpCustomHeaderField> customHeaderFields = null;

        public HttpRequestBuilder(Policy[] policies)
        {
            // Add Policies to be applied
            _policyWrapper = Policy.WrapAsync(policies);
            _client = new HttpClient();
        }

        private Task<T> HttpInvoker<T>(Func<Task<T>> action)
        {
            // Executes the action applying all
            // the polices defined in the wrapper
            return _policyWrapper.ExecuteAsync(() => action());
        }

        /// <summary>
        /// This method adds HTTP method (i.e. GET, PUT, POST, DELETE) to HTTP header
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddMethod(HttpMethod method)
        {
            this.method = method;
            return this;
        }

        /// <summary>
        /// This method adds request Uri to HTTP header
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddRequestUri(string requestUri)
        {
            this.requestUri = requestUri;
            return this;
        }

        /// <summary>
        /// This method adds HTTP content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddContent(HttpContent content)
        {
            this.content = content;
            return this;
        }

        /// <summary>
        /// This method adds bearer token to HTTP Request
        /// </summary>
        /// <param name="bearerToken">Bearer Token</param>
        /// <returns></returns>
        public HttpRequestBuilder AddBearerToken(string bearerToken)
        {
            this.bearerToken = bearerToken;
            return this;
        }

        /// <summary>
        /// This method adds accept header information to HTTP header i.e. application/json
        /// </summary>
        /// <param name="acceptHeader"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddAcceptHeader(string acceptHeader)
        {
            this.acceptHeader = acceptHeader;
            return this;
        }

        /// <summary>
        /// This method adds authentication header information to HTTP header
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddAuthenticationHeader(string username, string password)
        {
            this.username = username;
            this.password = password;
            return this;
        }

        /// <summary>
        /// This method adds timeout interval for HTTP request. By default, 15 seconds is set
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        /// <summary>
        /// This method add custom header fields to HTTP request i.e. ClientId: 781uw-hju901
        /// </summary>
        /// <param name="customHeaderFields"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddCustomHeaderFields(IEnumerable<HttpCustomHeaderField> customHeaderFields)
        {
            this.customHeaderFields = customHeaderFields;
            return this;
        }

        /// <summary>
        /// This method sends the HTTP request to the hosting server
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> SendAsync()
        {
            return HttpInvoker(async () =>
            {
                    // Setup request
                    var request = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(requestUri)
                };

                if (this.content != null)
                    request.Content = content;

                if (!string.IsNullOrEmpty(bearerToken))
                    request.Headers.Authorization =
                      new AuthenticationHeaderValue("Bearer", bearerToken);

                request.Headers.Accept.Clear();
                if (!string.IsNullOrEmpty(this.acceptHeader))
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
                request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    string credentials =
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
                    request.Headers.Add("Authorization", $"Basic {credentials}");
                }

                    //adding custom header fields
                    if (customHeaderFields != null)
                {
                    foreach (var headerField in customHeaderFields)
                    {
                        request.Headers.Add(headerField.HeaderName, headerField.HeaderValue);
                    }
                }

                var response = await _client.SendAsync(request);
                return response;
            });
        }
    }
}

