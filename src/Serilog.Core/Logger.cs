using System;
using System.Configuration;
using Serilog.Events;

namespace Serilog.Core
{
    public static class Logger
    {
        private static readonly ILogger perfLogger;
        private static readonly ILogger usageLogger;
        private static readonly ILogger errorLogger;
        private static readonly ILogger diagnosticLogger;

        static Logger()
        {
            perfLogger = new LoggerConfiguration()
                .WriteTo.File(path: "perf.txt")
                .CreateLogger();

            usageLogger = new LoggerConfiguration()
                .WriteTo.File(path: "usage.txt")
                .CreateLogger();

            errorLogger = new LoggerConfiguration()
                .WriteTo.File(path: "error.txt")
                .CreateLogger();

            diagnosticLogger = new LoggerConfiguration()
                .WriteTo.File(path: "diagnostic.txt")
                .CreateLogger();
        }

        public static void WritePerf(LogDetail infoLog)
        {
            perfLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoLog);
        }
        public static void WriteUsage(LogDetail infoLog)
        {
            usageLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoLog);
        }
        public static void WriteError(LogDetail infoLog)
        {
            errorLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoLog);
        }
        public static void WriteDiagnostic(LogDetail infoLog)
        {
            var writeDiagnostics = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDiagnostics"]);
            if (!writeDiagnostics)
            {
                return;
            }
            diagnosticLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoLog);
        }
    }
}
