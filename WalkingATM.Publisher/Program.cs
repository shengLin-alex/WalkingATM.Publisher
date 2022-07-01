using System.Security.Cryptography.X509Certificates;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.AspNetCore.Builder;
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

var webAppBuilder = WebApplication.CreateBuilder(args);
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

LogManager.Configuration = new NLogLoggingConfiguration(webAppBuilder.Configuration.GetSection("NLog"));
webAppBuilder.Logging.ClearProviders();
webAppBuilder.Logging.SetMinimumLevel(LogLevel.Trace);
webAppBuilder.Host.UseNLog();

webAppBuilder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
webAppBuilder.Services.AddOptions<AppSettings>().Bind(webAppBuilder.Configuration);
webAppBuilder.Services.AddSingleton<ITimeProvider, TimeProvider>();
webAppBuilder.Services.AddTransient<IStockPriceClientService, StockPriceClientService>();
webAppBuilder.Services.AddTransient<IStringEncodingConverter, StringEncodingConverter>();
webAppBuilder.Services.AddGrpcClient<StockPriceService.StockPriceServiceClient>(
        "StockPriceServiceClient",
        (servicesProvider, o) =>
        {
            var appSettings = servicesProvider.GetRequiredService<IOptions<AppSettings>>();
            o.Address = new Uri(appSettings.Value.LinebotGrpcHost);
        })
    .ConfigurePrimaryHttpMessageHandler(
        () =>
        {
            var httpClientHandler = new HttpClientHandler
            {
                // Validate the server certificate
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            // Pass the client certificate so the server can authenticate the client
            httpClientHandler.ClientCertificates.Add(
                X509Certificate2.CreateFromPemFile("certs/grpc.crt", "certs/grpc.key"));

            return httpClientHandler;
        });

webAppBuilder.Host.ConfigureContainer<ContainerBuilder>(
    builder =>
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
    });

await using var host = webAppBuilder.Build();
await host.RunAsync();