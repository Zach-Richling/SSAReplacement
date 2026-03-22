namespace SSAReplacement.Api.Domain;

public class JobStepParameter
{
    public long Id { get; set; }
    public long JobStepId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }

    public JobStep JobStep { get; set; } = null!;
}
