using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Opening;

public class OpeningRisingPushJob : PushLogDataJobBase
{
    public OpeningRisingPushJob(
        [KeyFilter(StrategyEnum.OpeningRising)]
        ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.OpeningRising)]
        IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<OpeningRisingPushJob> logger) : base(logFileMonitor, strategy, appSettings, logger)
    {
    }
}