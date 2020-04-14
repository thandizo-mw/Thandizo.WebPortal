using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.WebPortal.Services;

namespace Thandizo.WebPortal
{
    public static class Registrations
    {
        /// <summary>
        /// Registers domain services to the specified
        /// service descriptor
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            return services.AddScoped<IHttpRequestHandler, HttpRequestHandler>();
        }
    }
}
