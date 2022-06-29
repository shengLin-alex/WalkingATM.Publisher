using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.LogFileMonitor.Intraday;

public class IntradayFallLogFileMonitor : LogFileMonitorBase
{
    public IntradayFallLogFileMonitor(
        ILogger<IntradayFallLogFileMonitor> logger,
        IOptions<AppSettings> appSettings) : base(logger, appSettings)
    {
    }
}