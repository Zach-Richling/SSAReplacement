using Cronos;

namespace SSAReplacement.Api.Features.Schedules.Infrastructure;

public static class ScheduleHelpers
{
    /// <summary>
    /// Returns an error message if the cron expression is invalid; null if valid.
    /// Supports both 5-field (minute hour day month day-of-week) and 6-field (with seconds) expressions.
    /// </summary>
    public static string? ValidateCronExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return "Cron expression is required.";

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

    /// <summary>
    /// Returns the next UTC occurrence after <paramref name="utcNow"/> for a valid cron expression, or null if invalid or none.
    /// Supports 5-field and 6-field (with seconds) expressions, consistent with <see cref="ValidateCronExpression"/>.
    /// </summary>
    public static DateTime? TryGetNextOccurrenceUtc(string cronExpression, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
            return null;

        try
        {
            var parts = cronExpression.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var expr = parts.Length == 6
                ? CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds)
                : CronExpression.Parse(cronExpression);

            return expr.GetNextOccurrence(utcNow, TimeZoneInfo.Local);
        }
        catch (CronFormatException)
        {
            return null;
        }
    }
}
