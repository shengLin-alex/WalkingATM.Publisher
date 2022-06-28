using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.LogFileMonitor.Opening;

public class OpeningRisingLogFileMonitor : LogFileMonitorBase
{
    public OpeningRisingLogFileMonitor(ILogger<OpeningRisingLogFileMonitor> logger) : base(logger)
    {
    }
}