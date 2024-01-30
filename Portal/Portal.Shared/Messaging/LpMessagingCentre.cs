using System;


namespace Portal.Shared.Messaging;

public static class LpMessagingCentre
{
    public static LpMessage<Exception> Exception { get; } = new();
    // public static CoMessage<ApiException> ApiException { get; } = new();
    public static LpMessage<string> InfoMessage { get; } = new();
    public static LpMessage<string> WarningMessage { get; } = new();
    public static LpMessage<string> ErrorMessage { get; } = new();

    public static LpMessage<DateTime> GlobalPropertyChanged { get; } = new();
    // public static CoMessage<GlobalRequirements> GlobalRequirements { get; } = new();

    public static void UnsubscribeFromAll(object subscriber)
    {
        Exception.Unsubscribe(subscriber);
        //ApiException.Unsubscribe(subscriber);
        InfoMessage.Unsubscribe(subscriber);
        WarningMessage.Unsubscribe(subscriber);
        ErrorMessage.Unsubscribe(subscriber);
        //GlobalRequirements.Unsubscribe(subscriber);
    }
}