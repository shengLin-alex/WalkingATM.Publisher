using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayFallPushJob : PushLogDataJobBase
{
    public IntradayFallPushJob(
        [KeyFilter(StrategyEnum.IntradayFall)] ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.IntradayFall)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<IntradayFallPushJob> logger,
        ILifetimeScope lifetimeScope,
        ITimeProvider timeProvider) : base(
        lifetimeScope,
        logFileMonitor,
        strategy,
        appSettings,
        logger,
        timeProvider)
    {
    }
}