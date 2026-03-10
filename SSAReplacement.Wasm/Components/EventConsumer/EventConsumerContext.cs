namespace BlazorBlueprint.Components;

/// <summary>
/// Shared context cascaded from <see cref="BbEventConsumer"/> to its
/// <see cref="BbEventTemplate{TValue}"/> children. Manages event handler
/// registration and dispatch.
/// </summary>
public sealed class EventConsumerContext
{
    private readonly Dictionary<string, Action<string, string>> handlers = new(StringComparer.Ordinal);

    /// <summary>
    /// Current connection state of the underlying EventSource.
    /// </summary>
    public EventSourceState ConnectionState { get; internal set; } = EventSourceState.Disconnected;

    /// <summary>
    /// Consumer-level render mode.
    /// Overridden by <see cref="BbEventTemplate{TValue}.RenderMode"/> when set.
    /// </summary>
    public EventRenderMode RenderMode { get; internal set; } = EventRenderMode.Latest;

    /// <summary>
    /// Returns the registered event type names. Used by the parent component
    /// to tell the JS layer which named events to subscribe to.
    /// </summary>
    internal IReadOnlyCollection<string> RegisteredEventTypes => handlers.Keys;

    /// <summary>
    /// Registers a handler for a named SSE event type.
    /// </summary>
    internal void RegisterHandler(string eventType, Action<string, string> handler) =>
        handlers[eventType] = handler;

    /// <summary>
    /// Removes the handler for a named SSE event type.
    /// </summary>
    internal void UnregisterHandler(string eventType) =>
        handlers.Remove(eventType);

    /// <summary>
    /// Dispatches a raw JSON payload to the handler registered for
    /// <paramref name="eventType"/>. No-ops if no handler is registered.
    /// </summary>
    internal void DispatchEvent(string eventType, string eventId, string jsonData)
    {
        if (handlers.TryGetValue(eventType, out var handler))
        {
            handler(eventId, jsonData);
        }
    }
}
