namespace WalkingATM.Publisher.Utils;

public interface ICronTimerFactory
{
    ICronTimer CreateCronTimer(string cronExpression);
}

public class CronTimerFactory : ICronTimerFactory
{
    private readonly ITimeProvider _timeProvider;

    public CronTimerFactory(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public ICronTimer CreateCronTimer(string cronExpression)
    {
        return new CronTimer(
            cronExpression,
            _timeProvider);
    }
}