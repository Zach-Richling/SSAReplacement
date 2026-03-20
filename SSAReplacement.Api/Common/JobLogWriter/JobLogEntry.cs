namespace SSAReplacement.Api.Common.JobLogWriter;

public sealed record JobLogEntry(long JobRunId, string LogType, string Content, DateTime LogDate);
