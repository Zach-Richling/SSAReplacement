using CronExpressionDescriptor;

namespace SSAReplacement.Wasm.Extensions;

/// <summary>
/// Returns a human-readable description of a cron expression for display in the UI (e.g. tooltips).
/// </summary>
public static class CronDescription
{
    /// <summary>
    /// Returns a short English description of when the schedule runs, or a fallback message if the expression is invalid.
    /// </summary>
    public static string GetDescription(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return "No schedule.";

        try
        {
            return ExpressionDescriptor.GetDescription(expression.Trim());
        }
        catch
        {
            return "Invalid cron expression.";
        }
    }
}
