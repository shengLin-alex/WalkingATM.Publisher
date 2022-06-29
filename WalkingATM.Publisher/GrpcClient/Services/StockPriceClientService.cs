using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.GrpcClient.Exceptions;
using WalkingATM.Publisher.Strategies;

namespace WalkingATM.Publisher.GrpcClient.Services;

public interface IStockPriceClientService
{
    Task PushStockPrices(IEnumerable<string> stockPriceStrings, IStrategy strategy);
}

public class StockPriceClientService : IStockPriceClientService
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly StockPriceService.StockPriceServiceClient _stockPriceServiceClient;
    private StockPriceSource? _previousProcessed;

    public StockPriceClientService(IOptions<AppSettings> appSettings, GrpcClientFactory grpcClientFactory)
    {
        _appSettings = appSettings;
        _stockPriceServiceClient = grpcClientFactory.CreateClient<StockPriceService.StockPriceServiceClient>(
            _appSettings.Value.StockPriceServiceClient);
    }

    public async Task PushStockPrices(IEnumerable<string> stockPriceStrings, IStrategy strategy)
    {
        var prices = new List<StockPrice>();
        foreach (var stockPriceString in stockPriceStrings)
        {
            var s = new StockPriceSource(stockPriceString, _appSettings.Value.XQLogFileRecordSeparator);
            if (s.TimeOnly < strategy.StartTimeOnly)
                continue;

            if (StockPriceSource.StockPriceSourceComparer.Equals(s, _previousProcessed))
                continue;

            prices.Add(
                new StockPrice
                {
                    Strategy = s.Strategy,
                    Date = s.Date,
                    Time = s.Time,
                    Symbol = s.Symbol,
                    SymbolName = s.SymbolName,
                    Price = s.Price
                });

            _previousProcessed = s;
        }

        if (prices.Count == 0) return;

        var result = await _stockPriceServiceClient.PushStockPricesAsync(
            new StockPriceList
            {
                StockPrices = { prices }
            });

        if (result is not null && result.Code != Code.Success)
        {
            throw new CannotPushStockPrices($"Code:{result.Code}, Message:{result.Message}");
        }

        if (result is null)
        {
            throw new Exception("Something wrong with gRPC server.");
        }
    }
}