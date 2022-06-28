using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Opening;

public class OpeningRisingStopJob : StopPushJobBase
{
    public OpeningRisingStopJob(
        [KeyFilter(StrategyEnum.OpeningRising)]
        ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<OpeningRisingStopJob> logger) : base(logFileMonitor, lifetimeScope, appSettings, logger)
    {
    }
}