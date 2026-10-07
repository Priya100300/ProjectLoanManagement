using System.Text.Json.Serialization;

namespace ProjectLoanManagement.Models;

public class VideoRecording
{
    public int RecordingId { get; set; }
    public int VerificationId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public int DurationSeconds { get; set; }
    public string FileHashSha256 { get; set; }
    public DateTime RecordingStartTime { get; set; }
    public DateTime RecordingEndTime { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedDate { get; set; }

    /// <summary>Private storage key. Recordings are streamed through an authorised endpoint only.</summary>
    [JsonIgnore]
    public string FilePath { get; set; }
}
