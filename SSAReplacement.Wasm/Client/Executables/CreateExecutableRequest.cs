namespace SSAReplacement.Wasm.Client.Executables;

/// <summary>
/// Request body for POST /executables (create executable).
/// </summary>
public record CreateExecutableRequest(string Name);
