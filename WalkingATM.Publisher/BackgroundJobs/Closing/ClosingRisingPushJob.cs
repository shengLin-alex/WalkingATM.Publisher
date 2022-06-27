using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingRisingPushJob : PushLogDataJobBase
{
    public ClosingRisingPushJob(
        [KeyFilter(StrategyEnum.ClosingRising)] ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.ClosingRising)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<ClosingRisingPushJob> logger) : base(logFileMonitor, strategy, appSettings, logger)
    {
    }
}