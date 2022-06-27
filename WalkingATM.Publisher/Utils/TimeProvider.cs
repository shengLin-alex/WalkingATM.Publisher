namespace WalkingATM.Publisher.Utils;

public interface ITimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    bool IsWorkingDay();
}

public class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;

    public bool IsWorkingDay()
    {
        var twLocalTime = UtcNow.AddHours(8);
        return twLocalTime.DayOfWeek != DayOfWeek.Saturday && twLocalTime.DayOfWeek != DayOfWeek.Sunday;
    }
}