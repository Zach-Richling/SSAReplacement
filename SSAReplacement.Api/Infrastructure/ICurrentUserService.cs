namespace SSAReplacement.Api.Infrastructure;

public interface ICurrentUserService
{
    long? UserId { get; }
}
