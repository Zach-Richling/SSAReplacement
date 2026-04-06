namespace SSAReplacement.Api.Domain;

public interface ISoftDeletable
{
    byte RecStatus { get; set; }
    long? DeletedByUserId { get; set; }
    DateTime? DeletedDate { get; set; }
}
