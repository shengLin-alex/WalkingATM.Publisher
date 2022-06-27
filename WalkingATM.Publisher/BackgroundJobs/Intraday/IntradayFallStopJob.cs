using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;

namespace WalkingATM.Publisher.BackgroundJobs.Intraday;

public class IntradayFallStopJob : StopPushJobBase
{
    public IntradayFallStopJob(
        [KeyFilter(StrategyEnum.IntradayFall)] ILogFileMonitor logFileMonitor,
        IOptions<AppSettings> appSettings) : base(logFileMonitor, appSettings)
    {
    }
}