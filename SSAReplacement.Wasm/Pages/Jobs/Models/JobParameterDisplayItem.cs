using SSAReplacement.Wasm.Client.Executables;
using SSAReplacement.Wasm.Client.Jobs;

namespace SSAReplacement.Wasm.Pages.Jobs.Models;

public class JobParameterDisplayItem
{
    public JobParameter JobParameter { get; set; }
    public ExecutableParameter? ExecutableParameter { get; set; }

    public JobParameterDisplayItem(JobParameter? jobParam = null, ExecutableParameter? exeParameter = null)
    {
        if (jobParam is null && exeParameter is null)
        {
            throw new ArgumentNullException("Job or Executable parameter must be provided");
        }

        if (jobParam is not null && exeParameter is not null)
        {
            if (jobParam.Key != exeParameter.Name)
            {
                throw new ArgumentException("Job and Executable Parameters must have the same name.");
            }
        }

        if (jobParam is null && exeParameter is null)
        {
            throw new ArgumentNullException("Job or Executable parameter must be provided");
        }

        JobParameter = jobParam ?? new JobParameter() { Key = exeParameter!.Name };
        ExecutableParameter = exeParameter;
    }

    public string Key => JobParameter?.Key ?? ExecutableParameter?.Name!;
    public string? Value => string.IsNullOrEmpty(JobParameter?.Value) ? ExecutableParameter?.DefaultValue : JobParameter?.Value;

    public bool IsExecutableParameter => ExecutableParameter is not null;

    public string Type => ExecutableParameter?.TypeName ?? "Unknown";
    public string Description => ExecutableParameter?.Description ?? "";
    public bool IsRequired => ExecutableParameter?.Required ?? false;
}