using System.Diagnostics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;
using Serilog.Debugging;

namespace AspNetCoreWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // This line is just for debugging configuration issues.
            SelfLog.Enable(m => Debug.WriteLine(m));

            var config =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

            // This line uses Serilog.Settings.Configuration
            var configuration =
                new LoggerConfiguration()
                    .ReadFrom.Configuration(config);

            configuration
                // This allows you to use the PushProperty() functionality in Serilog.
                // For more details: https://github.com/serilog/serilog/wiki/Enrichment
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithMachineName()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId();

            Log.Logger = configuration.CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                   .UseSerilog();
    }
}