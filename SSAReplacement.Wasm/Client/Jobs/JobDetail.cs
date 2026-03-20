using SSAReplacement.Wasm.Client.Executables;
using SSAReplacement.Wasm.Client.Schedules;

namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Job detail (matches API JobDetailDto from GET /jobs/{id}).
/// </summary>
public class JobDetail
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? NotifyEmail { get; set; }
    public Executable Executable { get; set; } = new();
    public List<JobParameter> Variables { get; set; } = [];
    public List<Schedule> Schedules { get; set; } = [];
};
