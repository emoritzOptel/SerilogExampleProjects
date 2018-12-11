using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;

namespace AspNetCoreWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

            // This line uses Serilog.Settings.Configuration
            var configuration =
                new LoggerConfiguration()
                    .ReadFrom.Configuration(config);

            // This allows you to use the PushProperty() functionality in Serilog.
            // For more details: https://github.com/serilog/serilog/wiki/Enrichment
            configuration.Enrich.FromLogContext();

            Log.Logger = configuration.CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                   .UseSerilog();
    }
}