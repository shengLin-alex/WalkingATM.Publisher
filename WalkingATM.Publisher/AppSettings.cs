namespace WalkingATM.Publisher;

public class AppSettings
{
    public string PushLogDataJobCron { get; init; } = null!;
    public string StopPushJobCron { get; init; } = null!;
    public string TimeZoneId { get; init; } = null!;
    public string XQLogFileDateTimeFormat { get; init; } = null!;
    public string XQLogFileRecordSeparator { get; init; } = null!;
    public string XQLogFilePath { get; init; } = null!;
    public double LogFileMonitorTick { get; init; }
    public string LinebotGrpcHost { get; init; } = null!;
    public string StockPriceServiceClient { get; init; } = null!;
    public StrategySettings StrategySettings { get; init; } = null!;
    public string LogFileDelimiter { get; init; } = null!;
    public string LogFileEncoding { get; init; } = null!;
}

public class StrategySettings
{
    public string OpeningStartTime { get; init; } = null!;
    public string OpeningEndTime { get; init; } = null!;
    public TimeOnly OpeningStartTimeOnly => TimeOnly.Parse(OpeningStartTime);
    public TimeOnly OpeningEndTimeOnly => TimeOnly.Parse(OpeningEndTime);
    public string OpeningRisingStrategy { get; init; } = null!;
    public string OpeningFallStrategy { get; init; } = null!;
    public string IntradayStartTime { get; init; } = null!;
    public string IntradayEndTime { get; init; } = null!;
    public TimeOnly IntradayStartTimeOnly => TimeOnly.Parse(IntradayStartTime);
    public TimeOnly IntradayEndTimeOnly => TimeOnly.Parse(IntradayEndTime);
    public string IntradayRisingStrategy { get; init; } = null!;
    public string IntradayFallStrategy { get; init; } = null!;
    public string ClosingStartTime { get; init; } = null!;
    public string ClosingEndTime { get; init; } = null!;
    public TimeOnly ClosingStartTimeOnly => TimeOnly.Parse(ClosingStartTime);
    public TimeOnly ClosingEndTimeOnly => TimeOnly.Parse(ClosingEndTime);
    public string ClosingRisingStrategy { get; init; } = null!;
    public string ClosingFallStrategy { get; init; } = null!;
}