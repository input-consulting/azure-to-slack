using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Input.AzureToSlack.Extensions
{
    public static class HttpContextExtension
    {
        public static async Task ReplyWith(this HttpContext httpContext, int httpStatusCode, string message = null)
        {
            httpContext.Response.StatusCode = httpStatusCode;
            if (!string.IsNullOrEmpty(message))
            {
                await httpContext.Response.WriteAsync(message);
            }
        }
    }
}