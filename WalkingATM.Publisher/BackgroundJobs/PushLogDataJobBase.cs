using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs;

public abstract class PushLogDataJobBase : BackgroundService
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<PushLogDataJobBase> _logger;
    private readonly ILogFileMonitor _monitor;
    private readonly IStrategy _strategy;

    protected PushLogDataJobBase(
        ILogFileMonitor logFileMonitor,
        IStrategy strategy,
        IOptions<AppSettings> appSettings,
        ILogger<PushLogDataJobBase> logger)
    {
        _monitor = logFileMonitor;
        _strategy = strategy;
        _appSettings = appSettings;
        _logger = logger;
    }

    /// <summary>
    /// should execute at everyday 00:00
    /// </summary>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(Executing, stoppingToken);
        return Task.CompletedTask;
    }

    private async Task Executing()
    {
        using var timer = new CronTimer(_appSettings.Value.PushLogDataJobCron);
        while (await timer.WaitForNextTickAsync())
        {
            _monitor.OnLine += (_, e) =>
            {
                foreach (var line in e.Lines)
                {
                    // todo push data to line bot server
                    _logger.LogInformation("{Line}", line);
                }
            };

            var date = DateTime.Now.ToString(_appSettings.Value.XQLogFileDateTimeFormat);

            _monitor.Start(string.Format(_appSettings.Value.XQLogFilePath, _strategy.StrategyName, date));
        }
    }
}