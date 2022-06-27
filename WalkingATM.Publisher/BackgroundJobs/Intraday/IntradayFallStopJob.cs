using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayFallStopJob : StopPushJobBase
{
    public IntradayFallStopJob(
        [KeyFilter(StrategyEnum.IntradayFall)] ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<IntradayFallStopJob> logger) : base(logFileMonitor, lifetimeScope, appSettings, logger)
    {
    }
}