namespace SSAReplacement.Web.Client;

/// <summary>
/// Executable list item (matches API ExecutableDto from GET /executables).
/// </summary>
public record Executable(int Id, string? Name, DateTime CreatedAt);
