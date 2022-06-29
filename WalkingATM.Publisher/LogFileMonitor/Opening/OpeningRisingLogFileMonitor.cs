using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.LogFileMonitor.Opening;

public class OpeningRisingLogFileMonitor : LogFileMonitorBase
{
    public OpeningRisingLogFileMonitor(
        ILogger<OpeningRisingLogFileMonitor> logger,
        IOptions<AppSettings> appSettings) : base(logger, appSettings)
    {
    }
}