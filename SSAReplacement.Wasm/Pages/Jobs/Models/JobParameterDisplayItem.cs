using SSAReplacement.Wasm.Client.Executables;
using SSAReplacement.Wasm.Client.Jobs;

namespace SSAReplacement.Wasm.Pages.Jobs.Models;

public class JobParameterDisplayItem
{
    public JobStepParameter JobStepParameter { get; set; }
    public ExecutableParameter? ExecutableParameter { get; set; }

    public JobParameterDisplayItem(JobStepParameter? jobParam = null, ExecutableParameter? exeParameter = null)
    {
        if (jobParam is null && exeParameter is null)
        {
            throw new ArgumentNullException("Job step or Executable parameter must be provided");
        }

        if (jobParam is not null && exeParameter is not null)
        {
            if (jobParam.Key != exeParameter.Name)
            {
                throw new ArgumentException("Job step and Executable Parameters must have the same name.");
            }
        }

        JobStepParameter = jobParam ?? new JobStepParameter() { Key = exeParameter!.Name };
        ExecutableParameter = exeParameter;
    }

    public string Key => JobStepParameter?.Key ?? ExecutableParameter?.Name!;
    public string? Value
    {
        get => string.IsNullOrEmpty(JobStepParameter?.Value) ? ExecutableParameter?.DefaultValue : JobStepParameter?.Value;
        set
        {
            if (JobStepParameter is not null)
                JobStepParameter.Value = value;
        }
    }

    public bool IsExecutableParameter => ExecutableParameter is not null;

    public string Type => ExecutableParameter?.TypeName ?? "Unknown";
    public string Description => ExecutableParameter?.Description ?? "";
    public bool IsRequired => ExecutableParameter?.Required ?? false;
}
