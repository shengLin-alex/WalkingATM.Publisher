using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.LogFileMonitor.Intraday;

public class IntradayFallLogFileMonitor : LogFileMonitorBase
{
    public IntradayFallLogFileMonitor(ILogger<IntradayFallLogFileMonitor> logger) : base(logger)
    {
    }
}