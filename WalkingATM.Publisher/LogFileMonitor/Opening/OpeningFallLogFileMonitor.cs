using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.LogFileMonitor.Opening;

public class OpeningFallLogFileMonitor : LogFileMonitorBase
{
    public OpeningFallLogFileMonitor(
        ILogger<OpeningFallLogFileMonitor> logger,
        IOptions<AppSettings> appSettings) : base(logger, appSettings)
    {
    }
}