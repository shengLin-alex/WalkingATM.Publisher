using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayRisingStopJob : StopPushJobBase
{
    public IntradayRisingStopJob(
        [KeyFilter(StrategyEnum.IntradayRising)] ILogFileMonitor logFileMonitor,
        IOptions<AppSettings> appSettings) : base(
        logFileMonitor,
        appSettings)
    {
    }
}