using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.BackgroundJobs;

public class GrpcServerHealthCheckJob : BackgroundService
{
    private readonly ILogger<GrpcServerHealthCheckJob> _logger;
    private readonly StockPriceService.StockPriceServiceClient _stockPriceServiceClient;

    public GrpcServerHealthCheckJob(
        ILogger<GrpcServerHealthCheckJob> logger,
        IOptions<AppSettings> appSettings,
        GrpcClientFactory grpcClientFactory)
    {
        _logger = logger;
        _stockPriceServiceClient =
            grpcClientFactory.CreateClient<StockPriceService.StockPriceServiceClient>(
                appSettings.Value.StockPriceServiceClient);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(Executing, stoppingToken);
        return Task.CompletedTask;
    }

    private async Task Executing()
    {
        try
        {
            var result = await _stockPriceServiceClient.StockPricesHealthCheckAsync(new Empty());

            if (result is not null)
            {
                foreach (var stockPrice in result.StockPrices)
                {
                    _logger.LogInformation("{StockPrice}", stockPrice);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "{Message}, {StackTrace}", e.Message, e.StackTrace);
        }
    }
}