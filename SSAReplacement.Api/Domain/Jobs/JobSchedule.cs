namespace SSAReplacement.Api.Domain;

public class JobSchedule
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int ScheduleId { get; set; }

    public Job Job { get; set; } = null!;
    public Schedule Schedule { get; set; } = null!;
}
