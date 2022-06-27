using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Opening;

public class OpeningFallStopJob : StopPushJobBase
{
    public OpeningFallStopJob(
        [KeyFilter(StrategyEnum.OpeningFall)] ILogFileMonitor logFileMonitor,
        IOptions<AppSettings> appSettings) : base(logFileMonitor, appSettings)
    {
    }
}