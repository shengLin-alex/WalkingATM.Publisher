using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.LogFileMonitor.Opening;

public class OpeningFallLogFileMonitor : LogFileMonitorBase
{
    public OpeningFallLogFileMonitor(ILogger<OpeningFallLogFileMonitor> logger) : base(logger)
    {
    }
}