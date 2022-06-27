using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using WalkingATM.Publisher;
using WalkingATM.Publisher.BackgroundJobs.Closing;
using WalkingATM.Publisher.BackgroundJobs.Intraday;
using WalkingATM.Publisher.BackgroundJobs.Opening;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureServices(
        (hostingContext, services) =>
        {
            services.AddOptions<AppSettings>().Bind(hostingContext.Configuration);
            LogManager.Configuration = new NLogLoggingConfiguration(hostingContext.Configuration.GetSection("NLog"));
        })
    .ConfigureContainer<ContainerBuilder>(
        (_, containerBuilder) =>
        {
            containerBuilder.RegisterType<ClosingFallPushJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<ClosingFallStopJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<ClosingRisingPushJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<ClosingRisingStopJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<IntradayFallPushJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<IntradayFallStopJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<IntradayRisingPushJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<IntradayRisingStopJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<OpeningFallPushJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<OpeningFallStopJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<OpeningRisingPushJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            containerBuilder.RegisterType<OpeningRisingStopJob>()
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();

            containerBuilder.RegisterType<LogFileMonitor>()
                .Keyed<ILogFileMonitor>(StrategyEnum.ClosingFall)
                .SingleInstance();
            containerBuilder.RegisterType<LogFileMonitor>()
                .Keyed<ILogFileMonitor>(StrategyEnum.ClosingRising)
                .SingleInstance();
            containerBuilder.RegisterType<LogFileMonitor>()
                .Keyed<ILogFileMonitor>(StrategyEnum.IntradayFall)
                .SingleInstance();
            containerBuilder.RegisterType<LogFileMonitor>()
                .Keyed<ILogFileMonitor>(StrategyEnum.IntradayRising)
                .SingleInstance();
            containerBuilder.RegisterType<LogFileMonitor>()
                .Keyed<ILogFileMonitor>(StrategyEnum.OpeningFall)
                .SingleInstance();
            containerBuilder.RegisterType<LogFileMonitor>()
                .Keyed<ILogFileMonitor>(StrategyEnum.OpeningRising)
                .SingleInstance();

            containerBuilder.RegisterType<ClosingFallStrategy>()
                .Keyed<IStrategy>(StrategyEnum.ClosingFall)
                .SingleInstance();
            containerBuilder.RegisterType<ClosingRisingStrategy>()
                .Keyed<IStrategy>(StrategyEnum.ClosingRising)
                .SingleInstance();
            containerBuilder.RegisterType<IntradayFallStrategy>()
                .Keyed<IStrategy>(StrategyEnum.IntradayFall)
                .SingleInstance();
            containerBuilder.RegisterType<IntradayRisingStrategy>()
                .Keyed<IStrategy>(StrategyEnum.IntradayRising)
                .SingleInstance();
            containerBuilder.RegisterType<OpeningFallStrategy>()
                .Keyed<IStrategy>(StrategyEnum.OpeningFall)
                .SingleInstance();
            containerBuilder.RegisterType<OpeningRisingStrategy>()
                .Keyed<IStrategy>(StrategyEnum.OpeningRising)
                .SingleInstance();
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