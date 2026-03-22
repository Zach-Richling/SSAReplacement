using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Features.JobRuns.Domain;

public record JobLogDto(long Id, long JobRunStepId, string LogType, string Content, DateTime LogDate)
{
    public static JobLogDto From(JobLog l) => new(l.Id, l.JobRunStepId, l.LogType, l.Content, l.LogDate);
}
