namespace ProjectLoanManagement.Models;

public class KYCChecklist
{
    public int ChecklistId { get; set; }
    public int KYCVerificationId { get; set; }
    public int ChecklistMasterId { get; set; }
    public string ChecklistItem { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsVerified { get; set; }
    public string Remarks { get; set; }
    public int? VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }
}
