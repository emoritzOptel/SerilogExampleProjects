using System;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

namespace NetCoreConsoleApp
{
    public class Program
    {
        private static void Main()
        {
            SelfLog.Enable(Console.WriteLine);

            Log.Logger =
                new LoggerConfiguration()
                    .MinimumLevel.Is(LogEventLevel.Verbose)
                    .CreateLogger();
        }
    }
}