using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingFallPushJob : PushLogDataJobBase
{
    public ClosingFallPushJob(
        [KeyFilter(StrategyEnum.ClosingFall)] ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.ClosingFall)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<ClosingFallPushJob> logger,
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