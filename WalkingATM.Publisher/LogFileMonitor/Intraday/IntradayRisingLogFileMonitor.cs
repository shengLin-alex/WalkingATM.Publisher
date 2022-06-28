using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.LogFileMonitor.Intraday;

public class IntradayRisingLogFileMonitor : LogFileMonitorBase
{
    public IntradayRisingLogFileMonitor(ILogger<IntradayRisingLogFileMonitor> logger) : base(logger)
    {
    }
}