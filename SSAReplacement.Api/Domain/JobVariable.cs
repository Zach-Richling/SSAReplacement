namespace SSAReplacement.Api.Domain;

public class JobVariable
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }

    public Job Job { get; set; } = null!;
}
