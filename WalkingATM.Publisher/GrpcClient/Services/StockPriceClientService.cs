using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Options;
using WalkingATM.Publisher.GrpcClient.Exceptions;

namespace WalkingATM.Publisher.GrpcClient.Services;

public interface IStockPriceClientService
{
    Task PushStockPrices(IEnumerable<string> stockPriceStrings);
}

public class StockPriceClientService : IStockPriceClientService
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly GrpcClientFactory _grpcClientFactory;

    public StockPriceClientService(IOptions<AppSettings> appSettings, GrpcClientFactory grpcClientFactory)
    {
        _appSettings = appSettings;
        _grpcClientFactory = grpcClientFactory;
    }

    public async Task PushStockPrices(IEnumerable<string> stockPriceStrings)
    {
        var stockPriceServiceClient =
            _grpcClientFactory.CreateClient<Publisher.StockPriceService.StockPriceServiceClient>(
                _appSettings.Value.StockPriceServiceClient);

        var stockPriceList = new StockPriceList
        {
            StockPrices =
            {
                stockPriceStrings
                    .Select(s => new StockPriceSource(s, _appSettings.Value.XQLogFileRecordSeparator))
                    .Select(
                        stockPriceSource => new StockPrice
                        {
                            Strategy = stockPriceSource.Strategy,
                            Date = stockPriceSource.Date,
                            Time = stockPriceSource.Time,
                            Symbol = stockPriceSource.Symbol,
                            SymbolName = stockPriceSource.SymbolName,
                            Price = stockPriceSource.Price
                        })
                    .ToList()
            }
        };

        var result = await stockPriceServiceClient.PushStockPricesAsync(stockPriceList);
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