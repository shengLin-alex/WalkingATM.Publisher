using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingFallStopJob : StopPushJobBase
{
    public ClosingFallStopJob(
        [KeyFilter(StrategyEnum.ClosingFall)] ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<ClosingFallStopJob> logger) : base(logFileMonitor, lifetimeScope, appSettings, logger)
    {
    }
}