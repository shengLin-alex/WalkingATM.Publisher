using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Closing;

public class ClosingFallPushJob : PushLogDataJobBase
{
    public ClosingFallPushJob(
        [KeyFilter(StrategyEnum.ClosingFall)] ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.ClosingFall)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<ClosingFallPushJob> logger) : base(logFileMonitor, strategy, appSettings, logger)
    {
    }
}