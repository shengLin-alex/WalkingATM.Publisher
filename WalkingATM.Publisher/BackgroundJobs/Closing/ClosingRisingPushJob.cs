using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingRisingPushJob : PushLogDataJobBase
{
    public ClosingRisingPushJob(
        [KeyFilter(StrategyEnum.ClosingRising)]
        ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.ClosingRising)]
        IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<ClosingRisingPushJob> logger,
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