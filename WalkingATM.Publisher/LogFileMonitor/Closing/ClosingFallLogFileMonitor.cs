using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.LogFileMonitor.Closing;

public class ClosingFallLogFileMonitor : LogFileMonitorBase
{
    public ClosingFallLogFileMonitor(
        ILogger<ClosingFallLogFileMonitor> logger,
        IOptions<AppSettings> appSettings) : base(logger, appSettings)
    {
    }
}