namespace ProjectLoanManagement.Models;

public class KYCVerification
{
    public int KYCVerificationId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int? LoanId { get; set; }
    public string LoanNumber { get; set; }
    public string KYCType { get; set; }
    public string KYCStatus { get; set; }
    public DateTime? VerificationDate { get; set; }
    public int? VerifiedBy { get; set; }
    public string VerifiedByName { get; set; }
    public string RejectionReason { get; set; }
    public string Remarks { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>Full KYC case: header, documents, checklist and video sessions.</summary>
public class KYCDetails
{
    public KYCVerification Verification { get; set; }
    public List<KYCDocument> Documents { get; set; } = new List<KYCDocument>();
    public List<KYCChecklist> Checklist { get; set; } = new List<KYCChecklist>();
    public List<VideoVerification> VideoSessions { get; set; } = new List<VideoVerification>();
}
