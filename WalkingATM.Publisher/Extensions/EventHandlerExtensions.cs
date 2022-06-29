namespace WalkingATM.Publisher.Extensions;

public static class EventHandlerExtensions
{
    public static bool IsRegistered<TEventArg>(this EventHandler<TEventArg>? handler, Delegate prospectiveHandler)
    {
        return handler is not null &&
               handler.GetInvocationList().Any(existingHandler => existingHandler == prospectiveHandler);
    }

    public static bool IsRegistered<TEventArg>(this EventHandler<TEventArg>? handler)
    {
        return handler is not null;
    }
}