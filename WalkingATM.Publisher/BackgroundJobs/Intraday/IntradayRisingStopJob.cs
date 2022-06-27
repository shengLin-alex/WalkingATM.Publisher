using Autofac;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayRisingStopJob : StopPushJobBase
{
    public IntradayRisingStopJob(
        [KeyFilter(StrategyEnum.IntradayRising)] ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<IntradayRisingStopJob> logger) : base(
        logFileMonitor,
        lifetimeScope,
        appSettings,
        logger)
    {
    }
}