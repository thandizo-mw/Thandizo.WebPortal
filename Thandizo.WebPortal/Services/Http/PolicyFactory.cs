using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Polly;

namespace Thandizo.WebPortal.Services
{
    public class PolicyFactory
    {
        // Add jitter strategy to avoid multiple clients
        // from retrying Http requests at the same time
        private static Random jitter = new Random();
        /// <summary>
        /// Creates Circuit breaker and Wait and Retry policies
        /// with the specified logger
        /// </summary>
        /// <param name="logger">The event logger</param>
        /// <returns></returns>
        public static Policy[] CreatePolicies() =>
            new Policy[]
        {
            Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                // number of retries
                4,
                // exponential backoff
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                TimeSpan.FromMilliseconds(jitter.Next(0 , 100)),
                // on retry
                (exception, timeSpan, retryCount, context) =>
                {
                    var message = $"Retry {retryCount} implemented with Pollys" +
                    $"RetryPolicy " +
                    $"of {context.PolicyKey} " +
                    $"due to: {exception}.";

                }),
            Policy.Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                // number of exceptions before breaking circuit
                5,
                // time circuit opened before retry
                TimeSpan.FromMinutes(1),
                (exception, duration) =>
                {
                    // on circuit opened
                },
                () =>
                {
                    // on circuit closed

                })
        };
        public static Policy[] CreatePolicies(ILogger<HttpRequestBuilder> logger, Policy[] additionalPolicies)
        {
            var policies = new List<Policy>();
            policies.AddRange(CreatePolicies());
            policies.AddRange(additionalPolicies);
            return policies.ToArray();
        }
    }
}
