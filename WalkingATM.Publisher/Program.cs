using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using WalkingATM.Publisher;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder2 = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureServices(
        (hostingContext, services) =>
        {
            services.AddOptions<AppSettings>().Bind(hostingContext.Configuration);
            services.AddSingleton<ITimeProvider, TimeProvider>();

            services.AddGrpcClient<StockPriceService.StockPriceServiceClient>(
                    "StockPriceServiceClient",
                    (servicesProvider, o) =>
                    {
                        var appSettings = servicesProvider.GetRequiredService<IOptions<AppSettings>>();
                        o.Address = new Uri(appSettings.Value.LinebotGrpcHost);
                    })
                .ConfigurePrimaryHttpMessageHandler(
                    () =>
                    {
                        var httpClientHandler = new HttpClientHandler();

                        // Validate the server certificate
                        httpClientHandler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                        // Pass the client certificate so the server can authenticate the client
                        var clientCert = X509Certificate2.CreateFromPemFile("certs/grpc.crt", "certs/grpc.key");
                        httpClientHandler.ClientCertificates.Add(clientCert);

                        return httpClientHandler;
                    });

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

            // add new strategy:
            // 1. create new strategy push and stop job
            // 2. create new strategy class
            // 3. add new strategy value to enum
            // 4. add KeyFilter to new strategy push stop job
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

            containerBuilder.RegisterType<CronTimerFactory>()
                .As<ICronTimerFactory>()
                .InstancePerLifetimeScope();
        })
    .ConfigureLogging(
        logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);
        })
    .UseNLog();

using var host = builder2.Build();
await host.RunAsync();