namespace WalkingATM.Publisher;

public class AppSettings
{
    public string XQLogFileDateTimeFormat { get; init; } = null!;
    public string PushLogDataJobCron { get; init; } = null!;
    public string StopPushJobCron { get; init; } = null!;
    public string XQLogFilePath { get; init; } = null!;
}