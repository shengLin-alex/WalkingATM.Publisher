namespace WalkingATM.Publisher.Utils;

public interface ICronTimerFactory
{
    ICronTimer CreateCronTimer(string cronExpression);
}

public class CronTimerFactory : ICronTimerFactory
{
    public ICronTimer CreateCronTimer(string cronExpression)
    {
        return new CronTimer(cronExpression);
    }
}