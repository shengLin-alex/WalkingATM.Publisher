using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.LogFileMonitor.Closing;

public class ClosingRisingLogFileMonitor : LogFileMonitorBase
{
    public ClosingRisingLogFileMonitor(
        ILogger<ClosingRisingLogFileMonitor> logger,
        IOptions<AppSettings> appSettings) : base(logger, appSettings)
    {
    }
}