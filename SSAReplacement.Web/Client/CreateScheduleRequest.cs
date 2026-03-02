namespace SSAReplacement.Web.Client;

/// <summary>
/// Request body for POST /schedules (create schedule).
/// </summary>
public record CreateScheduleRequest(string? Name, string CronExpression, bool IsEnabled = true);
