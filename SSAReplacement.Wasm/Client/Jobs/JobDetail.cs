using SSAReplacement.Wasm.Client.Schedules;

namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Job detail (matches API JobDetailDto from GET /jobs/{id}).
/// </summary>
public class JobDetail
{
    public int Id { get; set; }
    public int? ExecutableId { get; set; }
    public string Name { get; set; } = "";
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? NotifyEmail { get; set; }
    public string? ExecutableName { get; set; }
    public List<JobVariable> Variables { get; set; } = [];
    public List<Schedule> Schedules { get; set; } = [];
};
