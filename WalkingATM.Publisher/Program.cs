using System.Security.Cryptography.X509Certificates;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using WalkingATM.Publisher;
using WalkingATM.Publisher.GrpcClient.Services;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration(
        builder =>
        {
            builder.AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables();
        })
    .ConfigureServices(
        (context, services) =>
        {
            services.AddOptions<AppSettings>().Bind(context.Configuration);
            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddTransient<IStockPriceClientService, StockPriceClientService>();
            services.AddTransient<IStringEncodingConverter, StringEncodingConverter>();
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
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            LogManager.Configuration = new NLogLoggingConfiguration(context.Configuration.GetSection("NLog"));
        })
    .ConfigureContainer<ContainerBuilder>(
        (_, builder) =>
        {
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => t.IsAssignableTo<IHostedService>())
                .Where(t => !t.IsAbstract)
                .As<IHostedService>()
                .WithAttributeFiltering()
                .SingleInstance();

            // add new strategy:
            // 1. create new strategy push and stop job
            // 2. create new strategy class
            // 3. create new LogFileMonitor class for new strategy
            // 3. add new strategy value to enum
            // 4. add KeyFilter to new strategy push stop job
            foreach (var strategyEnum in Enum.GetValues<StrategyEnum>())
            {
                builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                    .Where(t => t.IsAssignableTo<ILogFileMonitor>())
                    .Where(t => t.Name.StartsWith(strategyEnum.ToString()))
                    .Keyed<ILogFileMonitor>(strategyEnum)
                    .SingleInstance();

                builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                    .Where(t => t.IsAssignableTo<IStrategy>())
                    .Where(t => t.Name.StartsWith(strategyEnum.ToString()))
                    .Keyed<IStrategy>(strategyEnum)
                    .SingleInstance();
            }

            builder.RegisterType<CronTimerFactory>()
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

using var host = hostBuilder.Build();
await host.RunAsync();