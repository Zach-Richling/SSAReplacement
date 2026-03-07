namespace SSAReplacement.Wasm.Client.Jobs;

/// <summary>
/// Job variable (matches API JobVariableDto in JobDetailDto).
/// </summary>
public record JobVariable(string Key, string Value);
