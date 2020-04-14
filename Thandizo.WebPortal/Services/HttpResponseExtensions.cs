using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Services
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// This method deserializes the JSON object to .NET object type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static T ContentAsType<T>(this HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return string.IsNullOrEmpty(data) ?
                            default(T) :
                            JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// This method serializes the .NET object type to JSON object
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ContentAsJson(this HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// This method serializes the .NET object type to content string
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ContentAsString(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
