using Serilog.Core.Models;

namespace Serilog.Core
{
    public interface ICLogger
    {
        void Log(LogType type, LogDetail detail);
    }
}
