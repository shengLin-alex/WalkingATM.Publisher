using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs;

public class PushLogDataJob : BackgroundService
{
    private readonly ILogger<PushLogDataJob> _logger;
    private readonly ILogFileMonitor _monitor;

    public PushLogDataJob(ILogFileMonitor logFileMonitor, ILogger<PushLogDataJob> logger)
    {
        _monitor = logFileMonitor;
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
        using var timer = new CronTimer("2/2 * * * *");
        while (await timer.WaitForNextTickAsync())
        {
            _monitor.OnLine += (s, e) =>
            {
                foreach (var line in e.Lines)
                {
                    _logger.LogInformation("{Line}", line);
                }
            };

            // todo: maybe only need date
            var date = DateTime.Now.ToString("yyyyMMddHHmmss");

            // todo: change to real data path
            _monitor.Start(
                $"/Users/shenglin-alex/Workspace/WalkingATM.Publisher/WalkingATM.Publisher/test.{date}.txt");
        }
    }
}