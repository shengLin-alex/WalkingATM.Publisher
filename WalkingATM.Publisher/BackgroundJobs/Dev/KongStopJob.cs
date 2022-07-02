using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Dev;

public class KongStopJob : StopPushJobBase
{
    public KongStopJob(
        [KeyFilter(StrategyEnum.Kong)]
        ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<KongStopJob> logger) : base(logFileMonitor, lifetimeScope, appSettings, logger)
    {
    }
}