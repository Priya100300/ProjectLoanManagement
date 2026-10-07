using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

public class CreateVideoVerificationRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? KYCVerificationId { get; set; }

    /// <summary>User who will conduct the call (Verifier, LoanOfficer or Admin).</summary>
    [Required, Range(1, int.MaxValue)]
    public int? OfficerId { get; set; }

    /// <summary>UTC date and time, e.g. 2026-10-01T10:30:00Z</summary>
    [Required]
    public DateTime? ScheduledDate { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}

public class StartVideoRequest
{
    /// <summary>Room or session id from the video provider (WebRTC service).</summary>
    [StringLength(200)]
    public string ExternalSessionId { get; set; }
}

public class CompleteVideoRequest
{
    [Required, RegularExpression("^(Completed|Verified|Rejected)$", ErrorMessage = "Outcome must be Completed, Verified or Rejected.")]
    public string Outcome { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}
