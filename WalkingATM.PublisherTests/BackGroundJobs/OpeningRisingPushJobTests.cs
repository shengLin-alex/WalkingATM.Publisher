using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using WalkingATM.Publisher;
using WalkingATM.Publisher.BackgroundJobs.Opening;
using WalkingATM.Publisher.GrpcClient.Services;
using WalkingATM.Publisher.LogFileMonitor;
using WalkingATM.Publisher.Strategies;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.PublisherTests.BackGroundJobs;

[TestFixture]
public class OpeningRisingPushJobTests
{
    [SetUp]
    public void SetUp()
    {
        _logFileMonitor = Substitute.For<ILogFileMonitor>();
        _strategy = Substitute.For<IStrategy>();
        _options = Substitute.For<IOptions<AppSettings>>();
        _logger = Substitute.For<ILogger<OpeningRisingPushJob>>();
        _lifetimeScope = Substitute.For<ILifetimeScope>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _options.Value.Returns(
            new AppSettings
            {
                PushLogDataJobCron = CronExpression,
                StopPushJobCron = "1 * * * *",
                TimeZoneId = TimeZoneId,
                XQLogFileDateTimeFormat = "yyyyMMdd",
                XQLogFileRecordSeparator = "|",
                XQLogFilePath = "Data/{0}_{1}.log",
                LogFileMonitorTick = 0,
                LinebotGrpcHost = "https://www.bot.bluebloods.autos:1443",
                StockPriceServiceClient = "StockPriceServiceClient",
                StrategySettings = new StrategySettings
                {
                    OpeningRisingStrategy = "開盤上漲",
                    OpeningFallStrategy = "開盤下跌",
                    OpeningStartTime = "08:58:05",
                    OpeningEndTime = "09:10:00",
                    IntradayRisingStrategy = "盤中上漲",
                    IntradayFallStrategy = "盤中下跌",
                    IntradayStartTime = "09:43:05",
                    IntradayEndTime = "10:30:00",
                    ClosingRisingStrategy = "尾盤上漲",
                    ClosingFallStrategy = "尾盤下跌",
                    ClosingStartTime = "10:43:05",
                    ClosingEndTime = "12:30:00"
                }
            });

        _lifetimeScope.BeginLifetimeScope().Returns(_lifetimeScope);

        _componentRegistry = Substitute.For<IComponentRegistry>();
        _componentRegistry.TryGetServiceRegistration(Arg.Any<Service>(), out _).Returns(_ => true);
        _lifetimeScope.ComponentRegistry.Returns(_componentRegistry);

        _cronTimerFactory = Substitute.For<ICronTimerFactory>();
        _cronTimer = Substitute.For<ICronTimer>();
        _cronTimerFactory.CreateCronTimer(CronExpression).Returns(_cronTimer);
        _lifetimeScope
            .ResolveComponent(
                Arg.Is<ResolveRequest>(r => r.Service.Description == typeof(ICronTimerFactory).FullName))
            .Returns(_cronTimerFactory);

        _stockPriceClientService = Substitute.For<IStockPriceClientService>();
        _lifetimeScope
            .ResolveComponent(
                Arg.Is<ResolveRequest>(r => r.Service.Description == typeof(IStockPriceClientService).FullName))
            .Returns(_stockPriceClientService);

        _openingRisingPushJob = new OpeningRisingPushJob(
            _logFileMonitor,
            _strategy,
            _options,
            _logger,
            _lifetimeScope,
            _timeProvider);
    }

    private const string CronExpression = "* * * * *";
    private const string LineFromLogMonitor = "開盤下跌 | 2022/06/29 | 09:13:13 | 2615.TW | 萬海 | 價格 | 122.00 ";
    private const string TimeZoneId = "Asia/Taipei";
    private const int WaitForExecute = 200;
    private const string Line2FromLogMonitor = "Line2FromLogMonitor";

    private ILogFileMonitor _logFileMonitor;
    private IStrategy _strategy;
    private IOptions<AppSettings> _options;
    private ILogger<OpeningRisingPushJob> _logger;
    private ILifetimeScope _lifetimeScope;
    private ITimeProvider _timeProvider;
    private OpeningRisingPushJob _openingRisingPushJob;
    private ICronTimerFactory _cronTimerFactory;
    private ICronTimer _cronTimer;
    private IStockPriceClientService _stockPriceClientService;
    private IComponentRegistry _componentRegistry;

    [Test]
    public async Task Execute_Once()
    {
        _cronTimer.WaitForNextTickAsync(Arg.Any<CancellationToken>()).Returns(true, false); // first and second.

        _timeProvider.IsWorkingDay().Returns(true);

        _timeProvider.GetNowByTimeZoneId(TimeZoneId).Returns(new DateTime(2022, 06, 30));

        _strategy.StrategyName.Returns("OpeningRising");

        _logFileMonitor.OnLineCallback(
            Arg.Do<EventHandler<LogFileMonitorLineEventArgs>>(
                e =>
                {
                    e(
                        new object(),
                        new LogFileMonitorLineEventArgs
                        {
                            Lines = new[] { LineFromLogMonitor }
                        });
                }));

        await _openingRisingPushJob.StartAsync(CancellationToken.None);
        await Task.Delay(TimeSpan.FromMilliseconds(WaitForExecute));

        await _stockPriceClientService.Received(1)
            .PushStockPrices(
                Arg.Is<IEnumerable<string>>(i => i.ShouldEqual(new[] { LineFromLogMonitor })),
                _strategy,
                Arg.Any<CancellationToken>());

        _logFileMonitor.Received(1).Start("Data/OpeningRising_20220630.log");
    }

    [Test]
    public async Task Execute_Twice_With_Same_Line()
    {
        _cronTimer.WaitForNextTickAsync(Arg.Any<CancellationToken>()).Returns(true, false);

        _timeProvider.IsWorkingDay().Returns(true);

        _timeProvider.GetNowByTimeZoneId(TimeZoneId).Returns(new DateTime(2022, 06, 30));

        _strategy.StrategyName.Returns("OpeningRising");

        EventHandler<LogFileMonitorLineEventArgs> a = null;
        _logFileMonitor.OnLineCallback(
            Arg.Do<EventHandler<LogFileMonitorLineEventArgs>>(
                e => { a = e; }));

        await _openingRisingPushJob.StartAsync(CancellationToken.None);
        await Task.Delay(TimeSpan.FromMilliseconds(WaitForExecute));

        // first invoke
        a?.Invoke(
            new object(),
            new LogFileMonitorLineEventArgs
            {
                Lines = new[] { LineFromLogMonitor }
            });

        // second invoke
        a?.Invoke(
            new object(),
            new LogFileMonitorLineEventArgs()
            {
                Lines = new[] { Line2FromLogMonitor }
            });

        // should receive only 1
        await _stockPriceClientService.Received(1)
            .PushStockPrices(
                Arg.Is<IEnumerable<string>>(i => i.ShouldEqual(new[] { LineFromLogMonitor })),
                _strategy,
                Arg.Any<CancellationToken>());

        _logFileMonitor.Received(1).Start("Data/OpeningRising_20220630.log");
    }

    [Test]
    public async Task Execute_Twice_With_Diff_Line()
    {
        _cronTimer.WaitForNextTickAsync(Arg.Any<CancellationToken>()).Returns(true, false);

        _timeProvider.IsWorkingDay().Returns(true);

        _timeProvider.GetNowByTimeZoneId(TimeZoneId).Returns(new DateTime(2022, 06, 30));

        _strategy.StrategyName.Returns("OpeningRising");

        EventHandler<LogFileMonitorLineEventArgs> a = null;
        _logFileMonitor.OnLineCallback(
            Arg.Do<EventHandler<LogFileMonitorLineEventArgs>>(
                e => { a = e; }));

        await _openingRisingPushJob.StartAsync(CancellationToken.None);
        await Task.Delay(TimeSpan.FromMilliseconds(WaitForExecute));

        // first invoke
        a?.Invoke(
            new object(),
            new LogFileMonitorLineEventArgs
            {
                Lines = new[] { LineFromLogMonitor }
            });

        // second invoke
        a?.Invoke(
            new object(),
            new LogFileMonitorLineEventArgs()
            {
                Lines = new[] { Line2FromLogMonitor }
            });

        // should receive twice
        await _stockPriceClientService.Received(1)
            .PushStockPrices(
                Arg.Is<IEnumerable<string>>(i => i.ShouldEqual(new[] { LineFromLogMonitor })),
                _strategy,
                Arg.Any<CancellationToken>());
        
        await _stockPriceClientService.Received(1)
            .PushStockPrices(
                Arg.Is<IEnumerable<string>>(i => i.ShouldEqual(new[] { Line2FromLogMonitor })),
                _strategy,
                Arg.Any<CancellationToken>());

        _logFileMonitor.Received(1).Start("Data/OpeningRising_20220630.log");
    }
}