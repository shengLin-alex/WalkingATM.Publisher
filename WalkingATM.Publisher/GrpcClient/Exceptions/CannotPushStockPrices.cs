namespace WalkingATM.Publisher.GrpcClient.Exceptions;

public class CannotPushStockPrices : Exception
{
    public CannotPushStockPrices(string message) : base(message)
    {
    }
}