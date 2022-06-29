namespace WalkingATM.Publisher;

public class AppSettings
{
    public string XQLogFileDateTimeFormat { get; init; } = null!;
    public string PushLogDataJobCron { get; init; } = null!;
    public string StopPushJobCron { get; init; } = null!;
    public string XQLogFilePath { get; init; } = null!;
    public string LinebotGrpcHost { get; init; } = null!;
    public string StockPriceServiceClient { get; init; } = null!;
    public string XQLogFileRecordSeparator { get; init; } = null!;
    public double LogFileMonitorTick { get; init; }
}