using Input.AzureToSlack;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureToSlack
{
    public class Startup
    {
        private readonly string _slackUri;
        public IConfigurationRoot Configuration { get; set; }
        
        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
        
        public Startup()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
               
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            
            _slackUri = Configuration["Slack"];
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.UseAzureToSlack( _slackUri);
        }
    }
}