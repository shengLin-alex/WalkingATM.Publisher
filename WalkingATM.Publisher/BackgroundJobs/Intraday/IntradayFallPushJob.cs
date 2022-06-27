using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayFallPushJob : PushLogDataJobBase
{
    public IntradayFallPushJob(
        [KeyFilter(StrategyEnum.IntradayFall)] ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.IntradayFall)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<IntradayFallPushJob> logger) : base(logFileMonitor, strategy, appSettings, logger)
    {
    }
}