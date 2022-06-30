using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using WalkingATM.Publisher;
using WalkingATM.Publisher.GrpcClient;
using WalkingATM.Publisher.GrpcClient.Services;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.PublisherTests.GrpcClient;

[TestFixture]
public class StockPriceClientServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _options = Substitute.For<IOptions<AppSettings>>();
        _grpcClientFactory = Substitute.For<GrpcClientFactory>();

        _options.Value.Returns(
            new AppSettings
            {
                StockPriceServiceClient = "StockPriceServiceClient",
                XQLogFileRecordSeparator = "|",
                StrategySettings = new StrategySettings
                {
                    IntradayRisingStrategy = "盤中上漲",
                    IntradayStartTime = "09:43:05",
                    IntradayEndTime = "10:30:00"
                }
            });

        _stockPriceServiceClient = Substitute.For<StockPriceService.StockPriceServiceClient>();
        _grpcClientFactory
            .CreateClient<StockPriceService.StockPriceServiceClient>(_options.Value.StockPriceServiceClient)
            .Returns(_stockPriceServiceClient);

        _stockPriceClientService = new StockPriceClientService(_options, _grpcClientFactory);
    }

    private IOptions<AppSettings> _options;
    private GrpcClientFactory _grpcClientFactory;
    private StockPriceClientService _stockPriceClientService;
    private StockPriceService.StockPriceServiceClient _stockPriceServiceClient;

    [Test]
    public async Task PushStockPrices_IntradayRising()
    {
        _stockPriceServiceClient.PushStockPricesAsync(
                Arg.Is<StockPriceList>(
                    s => s.ShouldEqual(
                        new StockPriceList
                        {
                            StockPrices =
                            {
                                new List<StockPrice>
                                {
                                    new()
                                    {
                                        Strategy = "盤中上漲",
                                        Date = "2022/06/29",
                                        Time = "09:45:01",
                                        Symbol = "1795.TW",
                                        SymbolName = "美時",
                                        Price = "142.00"
                                    }
                                }
                            }
                        })),
                null,
                null,
                CancellationToken.None)
            .Returns(
                new AsyncUnaryCall<Res>(
                    Task.FromResult(new Res()
                    {
                        Code = "Success"
                    }),
                    o => Task.FromResult(new Metadata()),
                    _ => new Status(),
                    _ => new Metadata(),
                    _ => {},
                    new object()));

        var stockPriceClientResult = await _stockPriceClientService.PushStockPrices(
            new[]
            {
                "盤中上漲 | 2022/06/29 | 09:30:01 | 1795.TW | 美時 | 價格 | 141.00 ",
                "盤中上漲 | 2022/06/29 | 09:45:01 | 1795.TW | 美時 | 價格 | 142.00 ",
                "盤中上漲 | 2022/06/29 | 09:45:01 | 1795.TW | 美時 | 價格 | 143.00 "
            },
            new IntradayRisingStrategy(_options),
            CancellationToken.None);

        stockPriceClientResult.Should()
            .BeEquivalentTo(
                new StockPriceClientResult()
                {
                    Code = Code.Success
                });
    }
}