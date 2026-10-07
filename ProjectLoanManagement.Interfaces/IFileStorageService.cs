using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.Interfaces;

/// <summary>
/// Stores uploaded files outside the web root. Swap the implementation for Azure Blob / S3
/// without touching controllers or repositories.
/// </summary>
public interface IFileStorageService
{
    /// <param name="category">Folder group, e.g. "loan-documents", "kyc-documents", "video-recordings".</param>
    StoredFile Save(Stream content, string originalFileName, string contentType, string category);

    Stream OpenRead(string storageKey);

    void Delete(string storageKey);
}
