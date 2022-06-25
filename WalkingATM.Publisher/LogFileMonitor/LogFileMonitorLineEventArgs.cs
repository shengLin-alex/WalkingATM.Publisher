namespace WalkingATM.Publisher.LogFileMonitor;

public class LogFileMonitorLineEventArgs : EventArgs
{
    public string[] Lines { get; init; } = Array.Empty<string>();
}