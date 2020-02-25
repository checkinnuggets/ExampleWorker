using System;
using System.IO;
using System.Threading.Tasks;
using ExampleWorker.Extensions;
using ExampleWorker.Services;
using ExampleWorker.Services.Config;
using MessageProvider.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExampleWorker
{
    public class Program
    {
        // In 3.0 there is a template for this - https://gunnarpeipman.com/net/worker-service/  
        // Much of the stuff is there in 2.1, but you have to wire it up yourself...            
        private static IHost _host;
        
        public static async Task Main(string[] args)
        {
            _host = BuildHost(args);
            await _host.RunAsync();
        }

        private static IHost BuildHost(string[] args)
        {
            // this gives us the same kind of useful stuff we get in a web application
            // such as Configuration and DI.
            
            var config = GetConfiguration(Directory.GetCurrentDirectory(), args);
                       
            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    context.Configuration = config;
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();

                    // Host Options...
                    services.Configure<HostOptions>(option =>
                    {
                        // wait to allow services to shutdown gracefully before terminating
                        option.ShutdownTimeout = TimeSpan.FromSeconds(20);
                    });

                    // Configuration - strongly typed                                                         
                    services.AddOptions();
                    services.Configure<ServiceACfg>( config.GetSection<ServiceACfg>() );
                    services.Configure<ServiceBCfg>( config.GetSection(nameof(ServiceBCfg)) );

                    // List services - one host can run multiple services.
                    services.AddHostedService<MyServiceA>();
                    services.AddHostedService<MyServiceB>();

                    services.UseHelloWorldMessageProvider();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConfiguration(config.GetSection("Logging")); // <-- Read log configuration (such as Log Level to output) from config file.
                    logging.ClearProviders();   
                    logging.AddConsole();       // Here, we can add in Console, but also text file + there should be a provider for AWS.
                })
                .Build();

            return host;
        }

        private static IConfiguration GetConfiguration(string basePath, string[] args)
        {
            // Different providers added in the same way - environment, file, command line.
            // AWS: https://github.com/aws/aws-dotnet-extensions-configuration/
            // ...but may need a custom implementation to handle the key lookup stuff.

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)                                      
                .AddEnvironmentVariables()                                  // environment variables first
                .AddJsonFile("appsettings.json", optional: true)        // config files override environment 
                .AddCommandLine(args);                                      // command line params override all

            var config = configBuilder.Build();
            return config;
        }

    }
}
