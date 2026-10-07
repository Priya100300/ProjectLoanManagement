namespace ProjectLoanManagement.Models;

public class Loan
{
    public int LoanId { get; set; }
    public string LoanNumber { get; set; }
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerName { get; set; }
    public string LoanType { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TenureMonths { get; set; }
    public string Purpose { get; set; }
    public string LoanStatus { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public int? ApprovedBy { get; set; }
    public string ApprovedByName { get; set; }
    public DateTime? RejectionDate { get; set; }
    public string RejectionReason { get; set; }
    public DateTime? DisbursementDate { get; set; }
    public decimal? DisbursedAmount { get; set; }
    public string DisbursementMode { get; set; }
    public DateTime? ClosedDate { get; set; }
    public int? KYCVerificationId { get; set; }
    public string KYCStatus { get; set; }
    public int? AssessmentId { get; set; }
    public string AssessmentStatus { get; set; }
    public decimal? EligibleAmount { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class LoanStatusInfo
{
    public int LoanId { get; set; }
    public string LoanNumber { get; set; }
    public string LoanStatus { get; set; }
    public string KYCStatus { get; set; }
    public string AssessmentStatus { get; set; }
    public decimal TotalPayable { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
    public DateTime? NextDueDate { get; set; }
}
