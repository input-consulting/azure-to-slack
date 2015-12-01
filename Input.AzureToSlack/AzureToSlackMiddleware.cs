using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Input.AzureToSlack.Extensions;

namespace Input.AzureToSlack
{    
    public class AzureToSlackMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<AzureToSlackOptions> _optionsDelegate;
        private readonly AzureToSlackOptions _options;
        private readonly ILogger _log;

        public AzureToSlackMiddleware(RequestDelegate next, Action<AzureToSlackOptions> options, ILoggerFactory loggerFactory)
        {
            _next = next;
            _optionsDelegate = options;
            _options = new AzureToSlackOptions();
            _log = loggerFactory.CreateLogger("AzureToSlack");
        }

        public async Task Invoke(HttpContext context)
        {
            _optionsDelegate(_options);

            try
            {                
                if (IsValidAzureBuildRequest(context))
                {
                    var host = context.Request.Host.Value ?? "unkown host";
                    _log.LogInformation($"Incomming message from {host}");
                    
                    var body = await ReadBody<AzureDeployMessage>(context.Request.Body);
                    var channel = ReadChannel(context);

                    await PostAzureDeployMessageToSlack(body, channel);
                    await context.ReplyWith(200);
                    
                    _log.LogInformation("Successfully sent message to Slack");
                }
                else
                {
                    await context.ReplyWith(400);
                    _log.LogWarning("Not a valid Azure request");
                }
            }
            catch (System.Exception ex)
            {
                _log.LogError("Error", ex);
                await context.ReplyWith(500);
            }

            await _next(context);
        }

        private async Task PostAzureDeployMessageToSlack(AzureDeployMessage body, string channel = null)
        {            
            var success = (body.status == "success" && body.complete) ? "Published" : "Failed";
            var username = body.author;
            var message = $"Deploy by : {body.author} at {body.startTime.ToString()}";

            var attachment = new Attachment($"[{success}]")
            {
                PreText = $"[{success}] {body.message}",
                Color = (body.status == "success" && body.complete) ? "good" : "danger"
            };

            attachment.Fields = new List<Field> {
                new Field("Author") {Value = body.author, Short = true},
                new Field("Deployed by") {Value = body.deployer, Short = true},
                new Field("Site") {Value = body.siteName, Short = true},
            };

            if (!string.IsNullOrEmpty(body.statusText))
                attachment.Fields.Add(new Field("Status") { Value = body.statusText, Short = true });

            if (!string.IsNullOrEmpty(body.progress))
                attachment.Fields.Add(new Field("Progress") { Value = body.progress, Short = true });

            var slackClient = new SlackClient(_options.SlackUrl);
            await slackClient.PostMessageAsync(message, username, $"#{channel}", new List<Attachment>() { attachment });
        }

        private async Task<T> ReadBody<T>(Stream body)
        {
            var json = await body.ReadAll();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private string ReadChannel(HttpContext httpContext)
        {
            var channel = "general";
            if (httpContext.Request.Query.ContainsKey("channel"))
            {
                channel = httpContext.Request.Query["channel"];
            }
            return channel;
        }

        private bool IsValidAzureBuildRequest(HttpContext context)
        {
            return context.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase);
        }

    }

    internal class AzureDeployMessage
    {
        public string status { get; set; }
        public string statusText { get; set; }
        public bool complete { get; set; }
        public string progress { get; set; }
        public string author { get; set; }
        public string deployer { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string message { get; set; }
        public string siteName { get; set; }
        public string id { get; set; }
    }
}