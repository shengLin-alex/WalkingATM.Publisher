using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.BackgroundJobs.Opening;

public class OpeningFallPushJob : PushLogDataJobBase
{
    public OpeningFallPushJob(
        [KeyFilter(StrategyEnum.OpeningFall)] ILogFileMonitor logFileMonitor,
        [KeyFilter(StrategyEnum.OpeningFall)] IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<OpeningFallPushJob> logger) : base(logFileMonitor, strategy, appSettings, logger)
    {
    }
}