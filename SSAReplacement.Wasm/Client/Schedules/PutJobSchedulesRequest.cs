namespace SSAReplacement.Wasm.Client.Schedules;

/// <summary>
/// Request body for PUT /jobs/{id}/schedules.
/// </summary>
public record PutJobSchedulesRequest(IEnumerable<int>? ScheduleIds);
