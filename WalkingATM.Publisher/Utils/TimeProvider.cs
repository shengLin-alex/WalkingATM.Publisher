using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.Utils;

public interface ITimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTimeOffset NowOffset { get; }
    DateTimeOffset UtcNowOffset { get; }
    TimeZoneInfo TimeZoneByAppSetting { get; }
    bool IsWorkingDay();
    DateTime GetNowByTimeZoneId(string timeZoneId);
    TimeZoneInfo GetTimeZoneById(string timeZoneId);
}

public class TimeProvider : ITimeProvider
{
    private readonly IOptions<AppSettings> _appSettings;

    public TimeProvider(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTimeOffset NowOffset => DateTimeOffset.Now;
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    public TimeZoneInfo TimeZoneByAppSetting => TimeZoneInfo.FindSystemTimeZoneById(_appSettings.Value.TimeZoneId);

    public bool IsWorkingDay()
    {
        var twLocalTime = UtcNow.AddHours(8);
        return twLocalTime.DayOfWeek != DayOfWeek.Saturday && twLocalTime.DayOfWeek != DayOfWeek.Sunday;
    }

    public DateTime GetNowByTimeZoneId(string timeZoneId)
    {
        return TimeZoneInfo.ConvertTime(UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
    }

    public TimeZoneInfo GetTimeZoneById(string timeZoneId)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}