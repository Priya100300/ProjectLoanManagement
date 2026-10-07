namespace ProjectLoanManagement.Models;

public class VideoVerification
{
    public int VerificationId { get; set; }
    public int KYCVerificationId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int? LoanId { get; set; }
    public int OfficerId { get; set; }
    public string OfficerName { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string VerificationStatus { get; set; }
    public string ExternalSessionId { get; set; }
    public string Remarks { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<VideoRecording> Recordings { get; set; } = new List<VideoRecording>();
}
