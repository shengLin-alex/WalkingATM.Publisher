using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayRisingPushJob : PushLogDataJobBase
{
    public IntradayRisingPushJob(
        [KeyFilter(StrategyEnum.IntradayRising)]
        ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.IntradayRising)]
        IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<IntradayRisingPushJob> logger,
        ILifetimeScope lifetimeScope,
        ITimeProvider timeProvider,
        IHostEnvironment hostEnvironment) : base(
        lifetimeScope,
        logFileMonitor,
        strategy,
        appSettings,
        logger,
        timeProvider,
        hostEnvironment)
    {
    }
}