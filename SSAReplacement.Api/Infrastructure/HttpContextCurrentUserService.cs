namespace SSAReplacement.Api.Infrastructure;

public class HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public long? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
            return long.TryParse(value, out var id) ? id : null;
        }
    }
}
