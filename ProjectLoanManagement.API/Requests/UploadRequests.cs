using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.API.Requests;

// These live in the API project because IFormFile is an ASP.NET Core type.

public class UploadLoanDocumentRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? DocumentTypeId { get; set; }

    [Required]
    public IFormFile File { get; set; }
}

public class UploadKYCDocumentRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? DocumentTypeId { get; set; }

    /// <summary>Optional document number (PAN, passport...). Only a masked copy is stored.</summary>
    [StringLength(30)]
    public string DocumentNumber { get; set; }

    [Required]
    public IFormFile File { get; set; }
}

public class UploadRecordingRequest
{
    [Required]
    public IFormFile File { get; set; }

    [Required, Range(1, 86400)]
    public int? DurationSeconds { get; set; }

    /// <summary>Optional UTC start time from the video provider.</summary>
    public DateTime? RecordingStartTime { get; set; }

    /// <summary>Optional UTC end time from the video provider.</summary>
    public DateTime? RecordingEndTime { get; set; }
}
