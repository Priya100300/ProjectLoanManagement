namespace ProjectLoanManagement.Models;

public class LoanAssessment
{
    public int AssessmentId { get; set; }
    public int LoanId { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal ExistingMonthlyObligations { get; set; }
    public bool IsIncomeVerified { get; set; }
    public decimal? ProposedEMI { get; set; }
    public decimal? FOIRPercent { get; set; }
    public decimal? EligibleAmount { get; set; }
    public string RiskCategory { get; set; }
    public string AssessmentStatus { get; set; }
    public string Remarks { get; set; }
    public int AssessedBy { get; set; }
    public string AssessedByName { get; set; }
    public DateTime? AssessedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
