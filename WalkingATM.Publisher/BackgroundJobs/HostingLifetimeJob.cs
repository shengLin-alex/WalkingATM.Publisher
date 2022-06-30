using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WalkingATM.Publisher.BackgroundJobs;

public class HostingLifetimeJob : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<HostingLifetimeJob> _logger;

    public HostingLifetimeJob(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<HostingLifetimeJob> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(
            () =>
            {
                _logger.LogInformation(
                    "Application running on env: {Env}",
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            });

        return Task.CompletedTask;
    }
}