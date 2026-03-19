using SSAReplacement.Api.Features.Schedules.Handlers;

namespace SSAReplacement.Api.Features.Schedules;

public static class ScheduleEndpoints
{
    public static void MapScheduleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/schedules").WithTags("Schedules");

        group.MapGet("/", GetSchedules.Handler);
        group.MapGet("/{id:int}", GetScheduleById.Handler);
        group.MapPost("/", CreateSchedule.Handler);
        group.MapPut("/{id:int}", UpdateSchedule.Handler);
        group.MapDelete("/{id:int}", DeleteSchedule.Handler);
    }
}
