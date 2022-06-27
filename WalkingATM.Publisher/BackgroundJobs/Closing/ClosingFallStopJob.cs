using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingFallStopJob : StopPushJobBase
{
    public ClosingFallStopJob(
        [KeyFilter(StrategyEnum.ClosingFall)] ILogFileMonitor logFileMonitor,
        IOptions<AppSettings> appSettings) : base(logFileMonitor, appSettings)
    {
    }
}