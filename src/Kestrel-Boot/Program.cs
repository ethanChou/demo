using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;

namespace HttpBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder()
               .UseKestrel()
                .ConfigureKestrel((ctx, options) =>
                {
                    options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024;
                })
               .UseContentRoot(Directory.GetCurrentDirectory())
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   var env = hostingContext.HostingEnvironment;
                   config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                             optional: true, reloadOnChange: true);
                   config.AddEnvironmentVariables();
               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                   logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                   logging.AddConsole();
               })
               .UseUrls("http://*:8081")
               .UseStartup<Startup>()
               .Build()
               .Run();
        }
    }
}
