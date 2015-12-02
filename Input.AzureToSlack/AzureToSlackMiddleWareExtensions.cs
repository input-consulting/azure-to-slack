using Input.AzureToSlack;

namespace Microsoft.AspNet.Builder
{
    public static class AzureToSlackMiddlewareExtensions
    {
        public static IApplicationBuilder UseAzureToSlack(this IApplicationBuilder builder, string slackUrl) 
        {            
            return builder.UseAzureToSlack(new AzureToSlackOptions {SlackUrl = slackUrl});
        }
                
        public static IApplicationBuilder UseAzureToSlack(this IApplicationBuilder builder, AzureToSlackOptions options)
        {
            return builder.UseMiddleware<AzureToSlackMiddleware>(options);
        }
    }
}