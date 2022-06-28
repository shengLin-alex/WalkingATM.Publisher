using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingRisingStopJob : StopPushJobBase
{
    public ClosingRisingStopJob(
        [KeyFilter(StrategyEnum.ClosingRising)]
        ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<ClosingRisingStopJob> logger) : base(logFileMonitor, lifetimeScope, appSettings, logger)
    {
    }
}