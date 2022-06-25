using Microsoft.Extensions.Hosting;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs;

public class StopPushJob : BackgroundService
{
    private readonly ILogFileMonitor _monitor;

    public StopPushJob(ILogFileMonitor logFileMonitor)
    {
        _monitor = logFileMonitor;
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
        using var timer = new CronTimer("3/2 * * * *");
        while (await timer.WaitForNextTickAsync())
        {
            _monitor.Stop();
        }
    }
}