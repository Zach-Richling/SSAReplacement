namespace SSAReplacement.Wasm.Client.Schedules;

/// <summary>
/// Schedule list item (matches API ScheduleDto from GET /schedules).
/// </summary>
public class Schedule
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string CronExpression { get; set; } = "";
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}
