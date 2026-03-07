namespace SSAReplacement.Api.Services.JobLogWriter;

public sealed record JobLogEntry(int JobRunId, string LogType, string Content, DateTime LogDate);
