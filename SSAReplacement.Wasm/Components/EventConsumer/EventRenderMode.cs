namespace BlazorBlueprint.Components;

/// <summary>
/// Controls how <see cref="BbEventTemplate{TValue}"/> renders received events.
/// </summary>
public enum EventRenderMode
{
    /// <summary>
    /// Only the most recent event is rendered. Each new event replaces the previous one.
    /// </summary>
    Latest,

    /// <summary>
    /// All received events are accumulated and rendered as a list.
    /// </summary>
    Accumulate
}
