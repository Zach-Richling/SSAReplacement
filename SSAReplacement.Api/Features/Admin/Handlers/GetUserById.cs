using Facet.Extensions;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Admin.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Admin.Handlers;

public static class GetUserById
{
    public static async Task<IResult> Handler(long id, AppDbContext db)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.AuditEntries.OrderByDescending(a => a.OccurredAt))
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return Results.NotFound();

        return Results.Ok(user.ToFacet<User, UserDetailDto>());
    }
}
