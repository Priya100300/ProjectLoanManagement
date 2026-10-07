namespace ProjectLoanManagement.API.Services;

/// <summary>Bound from the "FileStorage" section of appsettings.json.</summary>
public class FileStorageOptions
{
    /// <summary>Folder OUTSIDE wwwroot. Relative paths are resolved from the app's content root.</summary>
    public string RootPath { get; set; } = "App_Data/Storage";
    public int MaxDocumentSizeMB { get; set; } = 10;
    public int MaxVideoSizeMB { get; set; } = 500;
    public string[] AllowedDocumentExtensions { get; set; } = { ".pdf", ".jpg", ".jpeg", ".png" };
    public string[] AllowedVideoExtensions { get; set; } = { ".mp4", ".webm" };
}
