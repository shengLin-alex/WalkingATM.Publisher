namespace WalkingATM.Publisher.Strategies;

public interface IStrategy
{
    string StrategyName { get; }
}

public class OpeningRisingStrategy : IStrategy
{
    public string StrategyName => "開盤上漲";
}

public class OpeningFallStrategy : IStrategy
{
    public string StrategyName => "開盤下跌";
}

public class IntradayRisingStrategy : IStrategy
{
    public string StrategyName => "盤中上漲";
}

public class IntradayFallStrategy : IStrategy
{
    public string StrategyName => "盤中下跌";
}

public class ClosingRisingStrategy : IStrategy
{
    public string StrategyName => "尾盤上漲";
}

public class ClosingFallStrategy : IStrategy
{
    public string StrategyName => "尾盤下跌";
}