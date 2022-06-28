using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.LogFileMonitor.Closing;

public class ClosingRisingLogFileMonitor : LogFileMonitorBase
{
    public ClosingRisingLogFileMonitor(ILogger<ClosingRisingLogFileMonitor> logger) : base(logger)
    {
    }
}