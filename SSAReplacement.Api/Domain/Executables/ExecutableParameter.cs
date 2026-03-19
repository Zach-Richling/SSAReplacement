namespace SSAReplacement.Api.Domain;

public class ExecutableParameter
{
    public long Id { get; set; }
    public int ExecutableVersionId { get; set; }
    public string Name { get; set; } = "";
    public string TypeName { get; set; } = "";
    public string? Description { get; set; }
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }

    public ExecutableVersion Version { get; set; } = null!;

}
