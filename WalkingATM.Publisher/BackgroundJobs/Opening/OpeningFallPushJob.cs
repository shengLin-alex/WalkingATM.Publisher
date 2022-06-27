using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs.Opening;

public class OpeningFallPushJob : PushLogDataJobBase
{
    public OpeningFallPushJob(
        [KeyFilter(StrategyEnum.OpeningFall)] ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.OpeningFall)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<OpeningFallPushJob> logger,
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