namespace SSAReplacement.Api.Domain;

public interface IAuditable
{
    long? CreatedByUserId { get; set; }
}
