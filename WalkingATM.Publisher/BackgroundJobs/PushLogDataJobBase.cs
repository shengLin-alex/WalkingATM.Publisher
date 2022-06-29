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
    private readonly object _syncRoot = new();
    private readonly ITimeProvider _timeProvider;
    private string[] _cachedLines = Array.Empty<string>();
    private bool _isPushed;

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
            var stockPriceClientService = serviceScope.Resolve<IStockPriceClientService>();
            var cronTimer = cronTimerFactory.CreateCronTimer(_appSettings.Value.PushLogDataJobCron);

            while (await cronTimer.WaitForNextTickAsync())
            {
                if (!_timeProvider.IsWorkingDay())
                {
                    continue;
                }

                _monitor.OnLineCallback(
                    (_, e) =>
                    {
                        lock (_syncRoot)
                        {
                            if (_isPushed && e.Lines.SequenceEqual(_cachedLines))
                                return;

                            if (_isPushed && !e.Lines.SequenceEqual(_cachedLines))
                                _isPushed = false;

                            if (_isPushed)
                                return;

                            // fire and forget
                            stockPriceClientService.PushStockPrices(e.Lines, _strategy);
                            foreach (var line in e.Lines)
                            {
                                _logger.LogInformation("{Line}", line);
                            }

                            _isPushed = true;
                            _cachedLines = e.Lines;
                        }
                    });

                var date = _timeProvider.GetNowByTimeZoneId(_appSettings.Value.TimeZoneId)
                    .ToString(_appSettings.Value.XQLogFileDateTimeFormat);

                _monitor.Start(string.Format(_appSettings.Value.XQLogFilePath, _strategy.StrategyName, date));
            }
        }
        catch (Exception e)
        {
            lock (_syncRoot)
            {
                _logger.LogCritical(e, "{Message}, {StackTrace}", e.Message, e.StackTrace);
            }
        }
    }
}