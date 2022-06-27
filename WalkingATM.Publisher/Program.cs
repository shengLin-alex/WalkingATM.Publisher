﻿using Autofac;
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
using WalkingATM.Publisher.Utils;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

// var builder = WebApplication.CreateBuilder(args);
//
// LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));
// builder.Logging.ClearProviders();
// builder.Logging.SetMinimumLevel(LogLevel.Trace);
// builder.Host.UseNLog();
//
// builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// builder.Services.AddOptions<AppSettings>().Bind(builder.Configuration);
// builder.Services.AddSingleton<ITimeProvider, ITimeProvider>();
// builder.Host.ConfigureContainer<ContainerBuilder>(
//     containerBuilder =>
//     {
//         containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
//             .Where(t => t.IsAssignableTo<IHostedService>())
//             .As<IHostedService>()
//             .WithAttributeFiltering()
//             .SingleInstance();
//
//         // add new strategy:
//         // 1. create new strategy push and stop job
//         // 2. create new strategy class
//         // 3. add new strategy value to enum
//         // 4. add KeyFilter to new strategy push stop job
//         foreach (var strategyEnum in Enum.GetValues<StrategyEnum>())
//         {
//             containerBuilder.RegisterType<LogFileMonitor>()
//                 .Keyed<ILogFileMonitor>(strategyEnum)
//                 .SingleInstance();
//
//             containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
//                 .Where(t => t.IsAssignableTo<IStrategy>())
//                 .Where(t => t.Name.StartsWith(strategyEnum.ToString()))
//                 .Keyed<IStrategy>(strategyEnum)
//                 .SingleInstance();
//         }
//
//         containerBuilder.RegisterType<CronTimerFactory>()
//             .As<ICronTimerFactory>()
//             .InstancePerLifetimeScope();
//     });


var builder2 = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureServices(
        (hostingContext, services) =>
        {
            services.AddOptions<AppSettings>().Bind(hostingContext.Configuration);
            services.AddSingleton<ITimeProvider, TimeProvider>();
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