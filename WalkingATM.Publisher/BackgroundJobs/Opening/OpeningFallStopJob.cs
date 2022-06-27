using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Opening;

public class OpeningFallStopJob : StopPushJobBase
{
    public OpeningFallStopJob(
        [KeyFilter(StrategyEnum.OpeningFall)] ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<OpeningFallStopJob> logger) : base(logFileMonitor, lifetimeScope, appSettings, logger)
    {
    }
}