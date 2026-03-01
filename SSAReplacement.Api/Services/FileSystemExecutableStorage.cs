using System.IO.Compression;

namespace SSAReplacement.Api.Services;

public interface IExecutableStorage
{
    string GetVersionDirectory(int executableId, int versionId);
    Task<string> SaveVersionAsync(int executableId, int versionId, Stream zipStream, CancellationToken cancellationToken = default);
}

public sealed class FileSystemExecutableStorage : IExecutableStorage
{
    private readonly string _rootPath;
    private readonly ILogger<FileSystemExecutableStorage> _logger;

    public FileSystemExecutableStorage(IConfiguration configuration, ILogger<FileSystemExecutableStorage> logger)
    {
        _rootPath = configuration["JobBinaries:Path"] ?? throw new Exception("Please define JobBinaries:Path environment variable.");
        _logger = logger;

        Directory.CreateDirectory(_rootPath);
    }

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
            _logger.LogInformation("Extracted executable binary for Executable {ExecutableId} Version {VersionId} to {Path}", executableId, versionId, versionDir);

            return versionDir;
        }
        finally
        {
            if (File.Exists(tempZip))
                File.Delete(tempZip);
        }
    }
}
