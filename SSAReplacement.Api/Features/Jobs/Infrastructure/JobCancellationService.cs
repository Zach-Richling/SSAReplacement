using System.Collections.Concurrent;

namespace SSAReplacement.Api.Features.Jobs.Infrastructure;

public sealed class JobCancellationService
{
    private readonly ConcurrentDictionary<long, CancellationTokenSource> _tokens = new();

    public void Register(long jobRunId, CancellationTokenSource cts)
        => _tokens[jobRunId] = cts;

    public void Unregister(long jobRunId)
        => _tokens.TryRemove(jobRunId, out _);

    public bool TryCancel(long jobRunId)
    {
        if (!_tokens.TryGetValue(jobRunId, out var cts))
            return false;

        cts.Cancel();
        return true;
    }
}
