using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs;

public abstract class StopPushJobBase : BackgroundService
{
    private readonly ILogFileMonitor _monitor;
    private readonly IOptions<AppSettings> _appSettings;

    protected StopPushJobBase(ILogFileMonitor logFileMonitor, IOptions<AppSettings> appSettings)
    {
        _monitor = logFileMonitor;
        _appSettings = appSettings;
    }

    /// <summary>
    /// should execute at everyday 23:59
    /// </summary>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(Executing, stoppingToken);
        return Task.CompletedTask;
    }

    private async Task Executing()
    {
        using var timer = new CronTimer(_appSettings.Value.StopPushJobCron);
        while (await timer.WaitForNextTickAsync())
        {
            _monitor.Stop();
        }
    }
}