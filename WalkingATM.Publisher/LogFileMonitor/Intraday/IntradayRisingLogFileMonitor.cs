using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.LogFileMonitor.Intraday;

public class IntradayRisingLogFileMonitor : LogFileMonitorBase
{
    public IntradayRisingLogFileMonitor(
        ILogger<IntradayRisingLogFileMonitor> logger,
        IOptions<AppSettings> appSettings) : base(logger, appSettings)
    {
    }
}