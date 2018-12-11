using System;
using Serilog;
using Serilog.Context;
using System.Diagnostics;
using Serilog.Events;

namespace Net46ConsoleApp
{
    public class Program
    {
        private static void Main()
        {
            Log.Logger =
                new LoggerConfiguration()
                    .ReadFrom.AppSettings("file")
                    .ReadFrom.AppSettings("console")
                    .MinimumLevel.Is(LogEventLevel.Verbose)
                    .Enrich.FromLogContext()
                    .CreateLogger();

            using (LogContext.PushProperty("ProcessName", Process.GetCurrentProcess().ProcessName))
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