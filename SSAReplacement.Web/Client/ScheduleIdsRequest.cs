namespace SSAReplacement.Web.Client;

/// <summary>
/// Request body for PUT /jobs/{id}/schedules.
/// </summary>
public record ScheduleIdsRequest(int[]? ScheduleIds);
