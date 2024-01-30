using Microsoft.Maui.Networking;
public static class ConnectionMonitor
{
    public static event EventHandler<bool> ConnectionStatusChanged;
    static ConnectionMonitor()
    {
        Connectivity.ConnectivityChanged += (s, e) =>
        {
            ConnectionStatusChanged?.Invoke(null, Connectivity.NetworkAccess == NetworkAccess.Internet);
        };
    }
    public static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
}
