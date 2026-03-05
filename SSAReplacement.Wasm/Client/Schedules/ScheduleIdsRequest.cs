namespace SSAReplacement.Wasm.Client.Schedules;

/// <summary>
/// Request body for PUT /jobs/{id}/schedules.
/// </summary>
public record ScheduleIdsRequest(int[]? ScheduleIds);
