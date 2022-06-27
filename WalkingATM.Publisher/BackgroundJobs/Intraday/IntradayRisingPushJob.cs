using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayRisingPushJob : PushLogDataJobBase
{
    public IntradayRisingPushJob(
        [KeyFilter(StrategyEnum.IntradayRising)]
        ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.IntradayRising)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<IntradayRisingPushJob> logger) : base(logFileMonitor, strategy, appSettings, logger)
    {
    }
}