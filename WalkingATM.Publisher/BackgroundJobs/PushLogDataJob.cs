using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs;

public class PushLogDataJob : BackgroundService
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<PushLogDataJob> _logger;
    private readonly ILogFileMonitor _monitor;

    public PushLogDataJob(
        ILogFileMonitor logFileMonitor,
        IOptions<AppSettings> appSettings,
        ILogger<PushLogDataJob> logger)
    {
        _monitor = logFileMonitor;
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

            _monitor.Start(string.Format(_appSettings.Value.XQLogFilePath, date));
        }
    }
}