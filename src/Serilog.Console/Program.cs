using System;
using Serilog.Core;

namespace Serilog.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var logDetail = GetLogDetail("Starting app", null);
            CLogger.WriteDiagnostic(logDetail);

            var tracker = new PerfTracker("FloggerConsole_Execution", "", logDetail.UserName, 
                logDetail.Location, logDetail.Product, logDetail.Layer);

            try
            {
                var ex = new Exception("Something bad has happened!");
                ex.Data.Add("input param", "nothing to see here");
                throw ex;
            }
            catch (Exception ex)
            {
                logDetail = GetLogDetail("", ex);
                CLogger.WriteError(logDetail);
            }

            logDetail = GetLogDetail("used flogging console", null);
            CLogger.WriteUsage(logDetail);

            logDetail = GetLogDetail("stopping app", null);
            CLogger.WriteDiagnostic(logDetail);

            tracker.Stop();
        }

        private static LogDetail GetLogDetail(string message, Exception ex)
        {
            return new LogDetail
            {
                Product = "Logger",
                Location = "Seriglog.Console",
                Layer = "Job",
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Message = message,
                Exception = ex
            };
        }
    }
}
