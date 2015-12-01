using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Input.AzureToSlack
{
    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly string _channel;

        protected SlackClient() {/* noop */}

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        public async Task PostMessageAsync(string text, string username = null, string channel = null, IEnumerable<Attachment> attachments = null)
        {
            var payload = BuildPayload(text, username, channel, attachments);
            await PostPayloadAsync(payload);
        }

        private Payload BuildPayload(string text, string username, string channel, IEnumerable<Attachment> attachments = null)
        {
            username = username ?? "";

            var payload = new Payload
            {
                Channel = channel,
                Username = username,
                Text = text,
                Attachments = attachments
            };

            return payload;
        }

        private HttpClient BuildHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("deflate"));

            return client;
        }

        private async Task PostPayloadAsync(Payload payload)
        {
            using (var client = BuildHttpClient())
            {
                var data = JsonConvert.SerializeObject(payload);
                var response = await client.PostAsync(_uri, new StringContent(data, Encoding.UTF8, "application/json"));
            }
        }
    }

    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }
    }

    public class Attachment
    {
        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        [JsonProperty("pretext")]
        public string PreText { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
        [JsonProperty("mrkdwn_in")]
        public List<string> MarkdownIn { get; private set; }

        public Attachment(string fallback)
        {
            Fallback = fallback;
            MarkdownIn = new List<string> { "fields" };
        }
    }

    public class Field
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("short")]
        public bool Short { get; set; }

        public Field(string title)
        {
            Title = title;
        }
    }
}

