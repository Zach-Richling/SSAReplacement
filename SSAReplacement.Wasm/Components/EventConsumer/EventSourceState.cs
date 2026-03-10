namespace BlazorBlueprint.Components;

/// <summary>
/// Represents the connection state of an EventSource.
/// </summary>
public enum EventSourceState
{
    Disconnected,
    Connecting,
    Open,
    Reconnecting,
    Closed
}
