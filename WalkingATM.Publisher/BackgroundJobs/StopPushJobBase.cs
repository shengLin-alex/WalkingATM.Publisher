using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.Publisher.BackgroundJobs;

public abstract class StopPushJobBase : BackgroundService
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ILogger<StopPushJobBase> _logger;
    private readonly ILogFileMonitor _monitor;

    protected StopPushJobBase(
        ILogFileMonitor logFileMonitor,
        ILifetimeScope lifetimeScope,
        IOptions<AppSettings> appSettings,
        ILogger<StopPushJobBase> logger)
    {
        _monitor = logFileMonitor;
        _lifetimeScope = lifetimeScope;
        _appSettings = appSettings;
        _logger = logger;
    }

    /// <summary>
    /// should execute at everyday after market close.
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
            var cronTimer = cronTimerFactory.CreateCronTimer(_appSettings.Value.StopPushJobCron);

            while (await cronTimer.WaitForNextTickAsync())
            {
                _monitor.Stop();
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "{Message}, {StackTrace}", e.Message, e.StackTrace);
        }
    }
}