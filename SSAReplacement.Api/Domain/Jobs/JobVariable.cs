namespace SSAReplacement.Api.Domain;

public class JobVariable
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }

    public Job Job { get; set; } = null!;
}
