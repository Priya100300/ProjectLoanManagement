using Microsoft.Extensions.Options;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Services;

/// <summary>Checks size, extension and the file's first bytes ("magic number") before anything is stored.</summary>
public class FileValidator
{
    private readonly FileStorageOptions _options;

    public FileValidator(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    public ResultSet ValidateDocument(IFormFile file)
    {
        return Validate(file, _options.AllowedDocumentExtensions, _options.MaxDocumentSizeMB, "DOC");
    }

    public ResultSet ValidateVideo(IFormFile file)
    {
        return Validate(file, _options.AllowedVideoExtensions, _options.MaxVideoSizeMB, "VIDEO");
    }

    public static string ContentTypeFor(string fileName)
    {
        switch (Path.GetExtension(fileName).ToLowerInvariant())
        {
            case ".pdf": return "application/pdf";
            case ".jpg":
            case ".jpeg": return "image/jpeg";
            case ".png": return "image/png";
            case ".mp4": return "video/mp4";
            case ".webm": return "video/webm";
            default: return "application/octet-stream";
        }
    }

    private static ResultSet Validate(IFormFile file, string[] allowedExtensions, int maxSizeMB, string module)
    {
        if (file == null || file.Length == 0)
        {
            return ResultSet.Failure("Please attach a non-empty file.", module + "_400");
        }

        if (file.Length > (long)maxSizeMB * 1024 * 1024)
        {
            return ResultSet.Failure("File is larger than the " + maxSizeMB + " MB limit.", module + "_413");
        }

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return ResultSet.Failure("File type not allowed. Allowed: " + string.Join(", ", allowedExtensions), module + "_415");
        }

        if (!HasExpectedSignature(file, extension))
        {
            return ResultSet.Failure("File content does not match its extension.", module + "_415");
        }

        return ResultSet.Success(null, "File is valid");
    }

    private static bool HasExpectedSignature(IFormFile file, string extension)
    {
        byte[] header = new byte[12];
        int read;
        using (Stream stream = file.OpenReadStream())
        {
            read = stream.Read(header, 0, header.Length);
        }

        if (read < 4)
        {
            return false;
        }

        switch (extension)
        {
            case ".pdf":
                return header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46;           // %PDF
            case ".jpg":
            case ".jpeg":
                return header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            case ".png":
                return header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
            case ".mp4":
                return read >= 8 && header[4] == 0x66 && header[5] == 0x74 && header[6] == 0x79 && header[7] == 0x70; // ....ftyp
            case ".webm":
                return header[0] == 0x1A && header[1] == 0x45 && header[2] == 0xDF && header[3] == 0xA3;
            default:
                return false;
        }
    }
}
