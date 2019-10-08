using System;
using System.Configuration;
using Serilog.Core.Models;
using Serilog.Events;

namespace Serilog.Core
{
    public class LoggerFile: ICLogger
    {
        private readonly ILogger perfLogger;
        private readonly ILogger usageLogger;
        private readonly ILogger errorLogger;
        private readonly ILogger diagnosticLogger;

        // todo: appsettings
        private const string pathLogFiles = "C:/Logs/BankTransfer/";

        public LoggerFile()
        {
            perfLogger = new LoggerConfiguration()
                .WriteTo.File($"{pathLogFiles}perf.txt")
                .CreateLogger();

            usageLogger = new LoggerConfiguration()
                .WriteTo.File($"{pathLogFiles}usage.txt")
                .CreateLogger();

            errorLogger = new LoggerConfiguration()
                .WriteTo.File($"{pathLogFiles}error.txt")
                .CreateLogger();

            diagnosticLogger = new LoggerConfiguration()
                .WriteTo.File($"{pathLogFiles}diagnostic.txt")
                .CreateLogger();
        }

        public void Log(LogType type, LogDetail detail)
        {
            switch (type)
            {
                case LogType.PERFORMANCE:
                    WritePerf(detail);
                    break;
                case LogType.USAGE:
                    WriteUsage(detail);
                    break;
                case LogType.ERROR:
                    WriteError(detail);
                    break;
                case LogType.DIAGNOSTIC:
                    WriteDiagnostic(detail);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void WritePerf(LogDetail infoLog)
        {
            perfLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoLog);
        }
        private void WriteUsage(LogDetail infoLog)
        {
            usageLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoLog);
        }
        private void WriteError(LogDetail infoLog)
        {
            if (infoLog.Exception != null)
            {
                infoLog.Message = GetMessageFromException(infoLog.Exception);
            }
            errorLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoLog);
        }

        private static string GetMessageFromException(Exception ex)
        {
            return ex.InnerException == null ? ex.Message : GetMessageFromException(ex.InnerException);
        }

        private void WriteDiagnostic(LogDetail infoLog)
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
