using CronExpressionDescriptor;
using Cronos;

namespace SSAReplacement.Wasm.Extensions;

public class CronHelper
{
    /// <summary>
    /// Returns a short English description of when the schedule runs
    /// </summary>
    public static string? GetDescription(string? expression)
    {
        if (expression is null)
        {
            return null;
        }

        try
        {
            return ExpressionDescriptor.GetDescription(expression.Trim());
        }
        catch
        {
            return null;
        }
    }

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
