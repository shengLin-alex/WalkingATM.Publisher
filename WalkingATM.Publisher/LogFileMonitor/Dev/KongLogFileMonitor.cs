using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.LogFileMonitor.Dev;

public class KongLogFileMonitor : LogFileMonitorBase
{
    public KongLogFileMonitor(ILogger<KongLogFileMonitor> logger, IOptions<AppSettings> appSettings) : base(
        logger,
        appSettings)
    {
    }
}