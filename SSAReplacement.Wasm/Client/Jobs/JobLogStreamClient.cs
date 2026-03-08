using Microsoft.JSInterop;

namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Receives JS callbacks from the log stream and forwards to subscribed handlers.
/// </summary>
internal sealed class JobLogStreamReceiver
{
    private readonly Func<JobLog, Task>? _onLog;
    private readonly Func<Task>? _onStreamEnd;
    private readonly Action<string>? _onStreamError;

    public JobLogStreamReceiver(
        Func<JobLog, Task>? onLog,
        Func<Task>? onStreamEnd,
        Action<string>? onStreamError)
    {
        _onLog = onLog;
        _onStreamEnd = onStreamEnd;
        _onStreamError = onStreamError;
    }

    [JSInvokable]
    public Task OnLog(JobLog log) => _onLog?.Invoke(log) ?? Task.CompletedTask;

    [JSInvokable]
    public Task OnStreamEnd() => _onStreamEnd?.Invoke() ?? Task.CompletedTask;

    [JSInvokable]
    public void OnStreamError(string message)
    {
        _onStreamError?.Invoke(message);
    }
}

/// <summary>
/// Wraps the job log SSE stream (EventSource) started via JS interop. Dispose to close the stream.
/// </summary>
public sealed class JobLogStreamHandle : IAsyncDisposable
{
    private readonly IJSObjectReference _module;
    private readonly IJSObjectReference _handle;
    private readonly IDisposable? _dotNetRef;
    private bool _disposed;

    internal JobLogStreamHandle(IJSObjectReference module, IJSObjectReference handle, IDisposable dotNetRef)
    {
        _module = module;
        _handle = handle;
        _dotNetRef = dotNetRef;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            await _handle.InvokeVoidAsync("close");
        }
        finally
        {
            _dotNetRef?.Dispose();
            await _handle.DisposeAsync();
            await _module.DisposeAsync();
        }
    }

    /// <summary>
    /// Starts the job log SSE stream. Subscribe via <paramref name="onLog"/>, <paramref name="onStreamEnd"/>, and <paramref name="onStreamError"/>.
    /// JS invokes these when events arrive; the client owns the receiver and disposes it with the handle.
    /// </summary>
    public static async Task<JobLogStreamHandle> StartAsync(
        IJSRuntime js,
        string url,
        Func<JobLog, Task>? OnLog = null,
        Func<Task>? OnStreamEnd = null,
        Action<string>? OnStreamError = null)
    {
        var receiver = new JobLogStreamReceiver(OnLog, OnStreamEnd, OnStreamError);
        var dotNetRef = DotNetObjectReference.Create(receiver);

        try
        {
            var module = await js.InvokeAsync<IJSObjectReference>("import", "./js/logStream.js");
            var handle = await module.InvokeAsync<IJSObjectReference>("startJobLogStream", url, dotNetRef);
            return new JobLogStreamHandle(module, handle, dotNetRef);
        }
        catch
        {
            dotNetRef.Dispose();
            throw;
        }
    }
}
