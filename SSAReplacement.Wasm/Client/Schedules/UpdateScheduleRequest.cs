namespace SSAReplacement.Wasm.Client.Schedules;

public record UpdateScheduleRequest
(
    string Name,
    string CronExpression,
    bool IsEnabled
);
