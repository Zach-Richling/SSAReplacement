using Facet.Extensions.EFCore;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Admin.Domain;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Admin.Handlers;

public static class GetUsers
{
    public static async Task<IResult> Handler(AppDbContext db)
    {
        var list = await db.Users
            .AsNoTracking()
            .OrderBy(u => u.Username)
            .ToFacetsAsync<User, UserDto>();

        return Results.Ok(list);
    }
}
