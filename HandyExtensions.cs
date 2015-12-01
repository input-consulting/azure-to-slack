using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace AzureToSlack {
    public static class HandyExtensions {
        public static async Task ReplyWith(this HttpContext httpContext, int httpStatusCode, string message = null)
        {
            httpContext.Response.StatusCode = httpStatusCode;
            if (!string.IsNullOrEmpty(message)) {
                await httpContext.Response.WriteAsync(message);    
            }
        }

        public static async Task<string> ReadAll(this Stream stream)
        {
            var sb = new StringBuilder();
            byte[] buffer = new byte[1024];
            var offset = 0;
            var read = 0;
           
            while ((read = await stream.ReadAsync(buffer, offset, buffer.Length)) != 0)
                sb.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

            return sb.ToString();
        }
    }	
}