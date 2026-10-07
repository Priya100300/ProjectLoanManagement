namespace ProjectLoanManagement.Models;

/// <summary>Result of saving an uploaded file to secure storage.</summary>
public class StoredFile
{
    public string StorageKey { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public string Sha256 { get; set; }
}
