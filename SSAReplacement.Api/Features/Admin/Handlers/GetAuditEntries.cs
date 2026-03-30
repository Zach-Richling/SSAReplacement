using Facet.Extensions.EFCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Admin.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Admin.Handlers;

public static class GetAuditEntries
{
    public static async Task<IResult> Handler(
        AppDbContext db,
        [FromQuery] string? entityName = null,
        [FromQuery] long? userId = null,
        [FromQuery] string? action = null)
    {
        var query = db.AuditEntries.AsNoTracking().AsQueryable();

        if (entityName is not null)
            query = query.Where(a => a.EntityName == entityName);

        if (userId is not null)
            query = query.Where(a => a.UserId == userId);

        if (action is not null)
            query = query.Where(a => a.Action == action);

        var list = await query
            .OrderByDescending(a => a.OccurredAt)
            .Take(500)
            .ToFacetsAsync<AuditEntry, AuditEntryDto>();

        return Results.Ok(list);
    }
}
