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
            containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => t.IsAssignableTo<IHostedService>())
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();
            
            foreach (var strategyEnum in Enum.GetValues<StrategyEnum>())
            {
                containerBuilder.RegisterType<LogFileMonitor>()
                    .Keyed<ILogFileMonitor>(strategyEnum)
                    .SingleInstance();

                containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
                    .Where(t => t.IsAssignableTo<IStrategy>())
                    .Where(t => t.Name.StartsWith(strategyEnum.ToString()))
                    .Keyed<IStrategy>(strategyEnum)
                    .SingleInstance();
            }
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