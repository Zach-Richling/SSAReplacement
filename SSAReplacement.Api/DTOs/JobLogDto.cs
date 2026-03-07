using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.DTOs;

public record JobLogDto(int Id, int JobRunId, string LogType, string Content, DateTime LogDate)
{
    public static JobLogDto From(JobLog l) => new(l.Id, l.JobRunId, l.LogType, l.Content, l.LogDate);
}
