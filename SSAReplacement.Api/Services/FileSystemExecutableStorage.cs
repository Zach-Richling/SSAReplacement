using System.IO.Compression;

namespace SSAReplacement.Api.Services;

public interface IExecutableStorage
{
    string GetVersionDirectory(int executableId, int versionId);
    Task<string> SaveVersionAsync(int executableId, int versionId, Stream zipStream, CancellationToken cancellationToken = default);
}

public sealed class FileSystemExecutableStorage(IConfiguration configuration, ILogger<FileSystemExecutableStorage> logger) : IExecutableStorage
{
    private readonly string _rootPath = configuration["Executables:Path"]!;

    public string GetVersionDirectory(int executableId, int versionId)
    {
        return Path.Combine(_rootPath, executableId.ToString(), versionId.ToString());
    }

    public async Task<string> SaveVersionAsync(int executableId, int versionId, Stream zipStream, CancellationToken cancellationToken = default)
    {
        var versionDir = GetVersionDirectory(executableId, versionId);
        Directory.CreateDirectory(versionDir);

        var tempZip = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
        try
        {
            await using (var fs = File.Create(tempZip))
                await zipStream.CopyToAsync(fs, cancellationToken);

            ZipFile.ExtractToDirectory(tempZip, versionDir, overwriteFiles: true);
            logger.LogInformation("Extracted executable binary for Executable {ExecutableId} Version {VersionId} to {Path}", executableId, versionId, versionDir);

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
