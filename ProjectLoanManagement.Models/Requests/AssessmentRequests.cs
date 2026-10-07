using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

public class CreateAssessmentRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? LoanId { get; set; }

    [Required, Range(typeof(decimal), "0", "100000000")]
    public decimal? MonthlyIncome { get; set; }

    [Range(typeof(decimal), "0", "100000000")]
    public decimal ExistingMonthlyObligations { get; set; }

    [Required]
    public bool? IsIncomeVerified { get; set; }

    [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "RiskCategory must be Low, Medium or High.")]
    public string RiskCategory { get; set; }

    [Required, RegularExpression("^(In Progress|Recommended|Not Recommended)$",
        ErrorMessage = "AssessmentStatus must be In Progress, Recommended or Not Recommended.")]
    public string AssessmentStatus { get; set; }

    [StringLength(1000)]
    public string Remarks { get; set; }
}
