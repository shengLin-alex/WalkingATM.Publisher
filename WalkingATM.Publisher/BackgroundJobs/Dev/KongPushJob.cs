using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs.Dev;

public class KongPushJob : PushLogDataJobBase
{
    public KongPushJob(
        ILifetimeScope lifeTimeScope,
        [KeyFilter(StrategyEnum.Kong)]
        ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.Kong)]
        IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<KongPushJob> logger,
        ITimeProvider timeProvider) : base(lifeTimeScope, logFileMonitor, strategy, appSettings, logger, timeProvider)
    {
    }
}