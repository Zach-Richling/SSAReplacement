namespace SSAReplacement.Wasm.Client.Schedules;

/// <summary>
/// Schedule list item (matches API ScheduleDto from GET /schedules).
/// </summary>
public record Schedule(
    int Id,
    string Name,
    string CronExpression,
    bool IsEnabled,
    DateTime CreatedAt);
