using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.GrpcClient.Services;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs;

public abstract class PushLogDataJobBase : BackgroundService
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ILogger<PushLogDataJobBase> _logger;
    private readonly ILogFileMonitor _monitor;
    private readonly IStrategy _strategy;
    private readonly ITimeProvider _timeProvider;

    protected PushLogDataJobBase(
        ILifetimeScope lifeTimeScope,
        ILogFileMonitor logFileMonitor,
        IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<PushLogDataJobBase> logger,
        ITimeProvider timeProvider)
    {
        _lifetimeScope = lifeTimeScope;
        _monitor = logFileMonitor;
        _strategy = strategy;
        _appSettings = appSettings;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// should execute at everyday before market open.
    /// </summary>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(Executing, stoppingToken);
        return Task.CompletedTask;
    }

    private async Task Executing()
    {
        try
        {
            await using var serviceScope = _lifetimeScope.BeginLifetimeScope();
            var cronTimerFactory = serviceScope.Resolve<ICronTimerFactory>();
            var cronTimer = cronTimerFactory.CreateCronTimer(_appSettings.Value.PushLogDataJobCron);

            while (await cronTimer.WaitForNextTickAsync())
            {
                if (!_timeProvider.IsWorkingDay())
                {
                    continue;
                }

                var stockPriceClientService = serviceScope.Resolve<IStockPriceClientService>();
                _monitor.OnLineCallback(
                    (_, e) =>
                    {
                        // todo: when program open a while, event may cause some bug...
                        // fire and forget?
                        stockPriceClientService.PushStockPrices(e.Lines);
                        foreach (var line in e.Lines)
                        {
                            _logger.LogInformation("{Line}", line);
                        }
                    });

                var date = DateTime.Now.ToString(_appSettings.Value.XQLogFileDateTimeFormat);

                _monitor.Start(string.Format(_appSettings.Value.XQLogFilePath, _strategy.StrategyName, date));
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "{Message}, {StackTrace}", e.Message, e.StackTrace);
        }
    }
}