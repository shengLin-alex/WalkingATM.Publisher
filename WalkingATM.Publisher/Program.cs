using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using WalkingATM.Publisher;
using WalkingATM.Publisher.BackgroundJobs;
using WalkingATM.Publisher.LogFileMonitor;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (hostingContext, services) =>
        {
            services.AddOptions<AppSettings>().Bind(hostingContext.Configuration);
            services.AddSingleton<ILogFileMonitor, LogFileMonitor>();
            services.AddHostedService<PushLogDataJob>();
            services.AddHostedService<StopPushJob>();
            services.AddHttpClient(
                    "Linebot",
                    config =>
                    {
                        config.Timeout = TimeSpan.FromSeconds(3);
                        config.BaseAddress = new Uri(hostingContext.Configuration.GetValue<string>("LinebotHost"));
                    })
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        MaxConnectionsPerServer = 10
                    });
            LogManager.Configuration = new NLogLoggingConfiguration(hostingContext.Configuration.GetSection("NLog"));
        })
    .ConfigureLogging(
        logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);
        })
    .UseNLog();

using var host = builder.Build();

await host.RunAsync();