using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingRisingStopJob : StopPushJobBase
{
    public ClosingRisingStopJob(
        [KeyFilter(StrategyEnum.ClosingRising)] ILogFileMonitor logFileMonitor,
        IOptions<AppSettings> appSettings) : base(logFileMonitor, appSettings)
    {
    }
}