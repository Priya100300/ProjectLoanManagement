using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Services;

/// <summary>
/// Saves files to a private folder with random names (category/yyyy/MM/guid.ext).
/// The original file name is kept only as metadata in the database.
/// Replace this class with an Azure Blob / S3 implementation for production.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IOptions<FileStorageOptions> options, IWebHostEnvironment environment)
    {
        string configured = options.Value.RootPath;
        _rootPath = Path.GetFullPath(Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(environment.ContentRootPath, configured));

        Directory.CreateDirectory(_rootPath);
    }

    public StoredFile Save(Stream content, string originalFileName, string contentType, string category)
    {
        string extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        DateTime now = DateTime.UtcNow;
        string storageKey = string.Join("/", category, now.ToString("yyyy"), now.ToString("MM"), Guid.NewGuid().ToString("N") + extension);
        string fullPath = ResolvePath(storageKey);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        using (FileStream target = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
        {
            content.CopyTo(target);
        }

        string sha256;
        using (FileStream saved = File.OpenRead(fullPath))
        {
            sha256 = Convert.ToHexString(SHA256.HashData(saved)).ToLowerInvariant();
        }

        return new StoredFile
        {
            StorageKey = storageKey,
            FileName = Path.GetFileName(originalFileName),
            ContentType = contentType,
            SizeBytes = new FileInfo(fullPath).Length,
            Sha256 = sha256
        };
    }

    public Stream OpenRead(string storageKey)
    {
        string fullPath = ResolvePath(storageKey);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Stored file not found.");
        }

        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public void Delete(string storageKey)
    {
        string fullPath = ResolvePath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    /// <summary>Blocks path traversal: the resolved path must stay inside the storage root.</summary>
    private string ResolvePath(string storageKey)
    {
        string fullPath = Path.GetFullPath(Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar)));
        string rootWithSeparator = _rootPath.EndsWith(Path.DirectorySeparatorChar) ? _rootPath : _rootPath + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Invalid storage key.");
        }

        return fullPath;
    }
}
