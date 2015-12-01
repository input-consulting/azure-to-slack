using System;
using Input.AzureToSlack;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.Builder
{
    public static class AzureToSlackMiddlewareExtensions
    {
        public static IApplicationBuilder UseAzureToSlack(this IApplicationBuilder builder, Action<AzureToSlackOptions> options)
        {
            var logger = builder.ApplicationServices.GetService(typeof(ILoggerFactory));
            return builder.Use(next => new AzureToSlackMiddleware(next, options, logger as ILoggerFactory).Invoke);
        }
    }
}