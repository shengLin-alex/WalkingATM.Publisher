using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.LogFileMonitor.Closing;

public class ClosingFallLogFileMonitor : LogFileMonitorBase
{
    public ClosingFallLogFileMonitor(ILogger<ClosingFallLogFileMonitor> logger) : base(logger)
    {
    }
}