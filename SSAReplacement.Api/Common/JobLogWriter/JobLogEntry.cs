namespace SSAReplacement.Api.Common.JobLogWriter;

public sealed record JobLogEntry(long JobRunStepId, string LogType, string Content, DateTime LogDate);
