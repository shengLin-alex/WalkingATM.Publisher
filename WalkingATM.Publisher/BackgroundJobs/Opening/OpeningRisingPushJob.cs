using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs.Opening;

public class OpeningRisingPushJob : PushLogDataJobBase
{
    public OpeningRisingPushJob(
        [KeyFilter(StrategyEnum.OpeningRising)]
        ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.OpeningRising)]
        IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<OpeningRisingPushJob> logger,
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