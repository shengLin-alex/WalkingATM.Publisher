using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.BackgroundJobs;

public class HostingLifetimeJob : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<HostingLifetimeJob> _logger;

    public HostingLifetimeJob(
        IHostApplicationLifetime hostApplicationLifetime,
        IHostEnvironment hostEnvironment,
        ILogger<HostingLifetimeJob> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(
            () =>
            {
                _logger.LogInformation(
                    "Application running on env: {Env}",
                    _hostEnvironment.EnvironmentName);
            });

        return Task.CompletedTask;
    }
}