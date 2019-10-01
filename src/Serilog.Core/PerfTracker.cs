using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Serilog.Core
{
    public class PerfTracker
    {
        private readonly Stopwatch sw;
        private readonly LogDetail infoLog;

        public PerfTracker(
            string name, 
            string userId, 
            string userName, 
            string location, 
            string product, 
            string layer)
        {
            sw = Stopwatch.StartNew();
            infoLog = new LogDetail
            {
                Message = name,
                UserId = userId,
                UserName = userName,
                Product = product,
                Layer = layer,
                Location = location,
                Hostname = Environment.MachineName
            };

            var beginTime = DateTime.Now;
            infoLog.AdditionalInfo = new Dictionary<string, object>
            {
                { "Started", beginTime.ToString(CultureInfo.InvariantCulture) }
            };
        }

        public PerfTracker(
            string name,
            string userId,
            string userName,
            string location,
            string product,
            string layer,
            Dictionary<string, object> perfParams)
        : this(name, userId, userName, location, product, layer)
        {
            foreach (var perfParam in perfParams)
            {
                infoLog.AdditionalInfo.Add("input-" + perfParam.Key, perfParam.Value);
            }
        }

        public void Stop()
        {
            sw.Stop();
            infoLog.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            Logger.WritePerf(infoLog);
        }
    }
}
