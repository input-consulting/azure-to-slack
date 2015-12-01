using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Input.AzureToSlack.Extensions
{
    public static class StreamExtension
    {
        public static async Task<string> ReadAll(this Stream stream)
        {
            var sb = new StringBuilder();
            byte[] buffer = new byte[1024];
            var offset = 0;
            var read = 0;

            while ((read = await stream.ReadAsync(buffer, offset, buffer.Length)) != 0)
                sb.Append(Encoding.UTF8.GetString(buffer, 0, read));

            return sb.ToString();
        }
    }
}