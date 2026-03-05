using Cronos;

namespace SSAReplacement.Wasm.Extensions;

/// <summary>
/// Validates cron expressions (5-field or 6-field). Returns an error message if invalid; null if valid.
/// </summary>
public static class CronValidation
{
    /// <summary>
    /// Returns an error message if the cron expression is invalid; null if valid.
    /// Supports both 5-field (minute hour day month day-of-week) and 6-field (with seconds) expressions.
    /// </summary>
    public static string? Validate(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return "Enter a cron expression.";

        try
        {
            var parts = expression.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 6)
                CronExpression.Parse(expression, CronFormat.IncludeSeconds);
            else
                CronExpression.Parse(expression);
            return null;
        }
        catch (CronFormatException ex)
        {
            return "Invalid cron expression: " + ex.Message;
        }
    }
}
