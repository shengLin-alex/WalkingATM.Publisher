using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.Strategies;

public interface IStrategy
{
    string StrategyName { get; }
    string StartUpTime { get; }
    TimeOnly StartTimeOnly { get; }
    string EndTime { get; }
}

public class OpeningRisingStrategy : IStrategy
{
    private readonly IOptions<AppSettings> _appSettings;

    public OpeningRisingStrategy(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string StrategyName => _appSettings.Value.StrategySettings.OpeningRisingStrategy;
    public string StartUpTime => _appSettings.Value.StrategySettings.OpeningStartTime;
    public TimeOnly StartTimeOnly => _appSettings.Value.StrategySettings.OpeningStartTimeOnly;
    public string EndTime => _appSettings.Value.StrategySettings.OpeningEndTime;
}

public class OpeningFallStrategy : IStrategy
{
    private readonly IOptions<AppSettings> _appSettings;

    public OpeningFallStrategy(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string StrategyName => _appSettings.Value.StrategySettings.OpeningFallStrategy;
    public string StartUpTime => _appSettings.Value.StrategySettings.OpeningStartTime;
    public TimeOnly StartTimeOnly => _appSettings.Value.StrategySettings.OpeningStartTimeOnly;
    public string EndTime => _appSettings.Value.StrategySettings.OpeningEndTime;
}

public class IntradayRisingStrategy : IStrategy
{
    private readonly IOptions<AppSettings> _appSettings;

    public IntradayRisingStrategy(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string StrategyName => _appSettings.Value.StrategySettings.IntradayRisingStrategy;
    public string StartUpTime => _appSettings.Value.StrategySettings.IntradayStartTime;
    public TimeOnly StartTimeOnly => _appSettings.Value.StrategySettings.IntradayStartTimeOnly;
    public string EndTime => _appSettings.Value.StrategySettings.IntradayEndTime;
}

public class IntradayFallStrategy : IStrategy
{
    private readonly IOptions<AppSettings> _appSettings;

    public IntradayFallStrategy(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string StrategyName => _appSettings.Value.StrategySettings.IntradayFallStrategy;
    public string StartUpTime => _appSettings.Value.StrategySettings.IntradayStartTime;
    public TimeOnly StartTimeOnly => _appSettings.Value.StrategySettings.IntradayStartTimeOnly;
    public string EndTime => _appSettings.Value.StrategySettings.IntradayEndTime;
}

public class ClosingRisingStrategy : IStrategy
{
    private readonly IOptions<AppSettings> _appSettings;

    public ClosingRisingStrategy(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string StrategyName => _appSettings.Value.StrategySettings.ClosingRisingStrategy;
    public string StartUpTime => _appSettings.Value.StrategySettings.ClosingStartTime;
    public TimeOnly StartTimeOnly => _appSettings.Value.StrategySettings.ClosingStartTimeOnly;
    public string EndTime => _appSettings.Value.StrategySettings.ClosingEndTime;
}

public class ClosingFallStrategy : IStrategy
{
    private readonly IOptions<AppSettings> _appSettings;

    public ClosingFallStrategy(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string StrategyName => _appSettings.Value.StrategySettings.ClosingFallStrategy;
    public string StartUpTime => _appSettings.Value.StrategySettings.ClosingStartTime;
    public TimeOnly StartTimeOnly => _appSettings.Value.StrategySettings.ClosingStartTimeOnly;
    public string EndTime => _appSettings.Value.StrategySettings.ClosingEndTime;
}