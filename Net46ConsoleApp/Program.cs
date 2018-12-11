using Serilog;
using Serilog.Context;
using Serilog.Debugging;
using Serilog.Events;
using System;

namespace Net46ConsoleApp
{
    public class Program
    {
        private static void Main()
        {
            SelfLog.Enable(Console.WriteLine);

            Log.Logger =
                new LoggerConfiguration()
                    .MinimumLevel.Is(LogEventLevel.Verbose)
                    .ReadFrom.AppSettings("console")
                    .ReadFrom.AppSettings("file")
                    .ReadFrom.AppSettings("es")
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .CreateLogger();

            using (LogContext.PushProperty("CustomPropertyName", "CustomPropertyValue"))
            {
                Log.Verbose("Verbose log entry.");
                Log.Debug("Debug log entry.");
                Log.Information("Information log entry.");
                Log.Warning("Warning log entry.");
                Log.Error("Error log entry.");
                Log.Fatal("Fatal log entry.");
            }

            Console.WriteLine("Any key to exit.");
            Console.ReadKey();
        }
    }
}