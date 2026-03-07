namespace SSAReplacement.Api.Domain;

public class Executable
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ExecutableVersion> Versions { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
}
