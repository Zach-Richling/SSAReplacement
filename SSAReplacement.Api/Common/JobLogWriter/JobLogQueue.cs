using System.Threading.Channels;

namespace SSAReplacement.Api.Common.JobLogWriter;

public interface IJobLogQueue
{
    ValueTask EnqueueAsync(JobLogEntry entry, CancellationToken cancellationToken = default);
}

public sealed class JobLogQueue : IJobLogQueue
{
    private readonly Channel<JobLogEntry> _channel = Channel.CreateUnbounded<JobLogEntry>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    public ChannelReader<JobLogEntry> Reader => _channel.Reader;

    public ValueTask EnqueueAsync(JobLogEntry entry, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(entry, cancellationToken);
}
