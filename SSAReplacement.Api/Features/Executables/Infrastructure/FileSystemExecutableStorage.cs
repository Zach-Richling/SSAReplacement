using System.IO.Compression;

namespace SSAReplacement.Api.Features.Executables.Infrastructure;

public interface IExecutableStorage
{
    string GetVersionDirectory(long executableId, int versionNumber);
    Task<string> SaveVersionAsync(long executableId, int versionNumber, Stream zipStream, CancellationToken cancellationToken = default);
}

public sealed class FileSystemExecutableStorage(IConfiguration configuration, ILogger<FileSystemExecutableStorage> logger) : IExecutableStorage
{
    private readonly string _rootPath = configuration["Executables:Path"]!;

    public string GetVersionDirectory(long executableId, int versionNumber)
    {
        return Path.Combine(_rootPath, executableId.ToString(), versionNumber.ToString());
    }

    public async Task<string> SaveVersionAsync(long executableId, int versionNumber, Stream zipStream, CancellationToken cancellationToken = default)
    {
        var versionDir = GetVersionDirectory(executableId, versionNumber);
        Directory.CreateDirectory(versionDir);

        var tempZip = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
        try
        {
            await using (var fs = File.Create(tempZip))
                await zipStream.CopyToAsync(fs, cancellationToken);

            ZipFile.ExtractToDirectory(tempZip, versionDir, overwriteFiles: true);
            logger.LogInformation("Extracted executable binary for Executable {ExecutableId} Version {VersionNumber} to {Path}", executableId, versionNumber, versionDir);

            var devAppSettingsPath = Path.Combine(versionDir, "appsettings.Development.json");
            if (File.Exists(devAppSettingsPath))
            {
                logger.LogInformation("Removing appsettings.Development.json");
                File.Delete(devAppSettingsPath);
            }

            return versionDir;
        }
        finally
        {
            if (File.Exists(tempZip))
                File.Delete(tempZip);
        }
    }
}
