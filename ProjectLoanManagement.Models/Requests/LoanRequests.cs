using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

public class CreateLoanRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? CustomerId { get; set; }

    [Required, RegularExpression("^(Personal|Home|Vehicle|Business|Education|Gold)$",
        ErrorMessage = "LoanType must be Personal, Home, Vehicle, Business, Education or Gold.")]
    public string LoanType { get; set; }

    [Required, Range(typeof(decimal), "1000", "1000000000")]
    public decimal? LoanAmount { get; set; }

    [Required, Range(typeof(decimal), "0.01", "60")]
    public decimal? InterestRate { get; set; }

    [Required, Range(1, 360)]
    public int? TenureMonths { get; set; }

    [StringLength(300)]
    public string Purpose { get; set; }
}

public class UpdateLoanRequest
{
    [Required, RegularExpression("^(Personal|Home|Vehicle|Business|Education|Gold)$",
        ErrorMessage = "LoanType must be Personal, Home, Vehicle, Business, Education or Gold.")]
    public string LoanType { get; set; }

    [Required, Range(typeof(decimal), "1000", "1000000000")]
    public decimal? LoanAmount { get; set; }

    [Required, Range(typeof(decimal), "0.01", "60")]
    public decimal? InterestRate { get; set; }

    [Required, Range(1, 360)]
    public int? TenureMonths { get; set; }

    [StringLength(300)]
    public string Purpose { get; set; }
}

public class ApproveLoanRequest
{
    /// <summary>Leave empty to approve the requested amount.</summary>
    [Range(typeof(decimal), "1", "1000000000")]
    public decimal? ApprovedAmount { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}

public class RejectLoanRequest
{
    [Required, StringLength(500, MinimumLength = 5)]
    public string Reason { get; set; }
}

public class DisburseLoanRequest
{
    [Required, RegularExpression("^(NEFT|RTGS|IMPS|Cheque|Other)$", ErrorMessage = "DisbursementMode must be NEFT, RTGS, IMPS, Cheque or Other.")]
    public string DisbursementMode { get; set; }

    /// <summary>Full account number; only the last 4 digits are stored.</summary>
    [RegularExpression(@"^[0-9]{9,18}$", ErrorMessage = "BeneficiaryAccountNo must be 9-18 digits.")]
    public string BeneficiaryAccountNo { get; set; }

    [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "IFSCCode format is ABCD0123456.")]
    public string IFSCCode { get; set; }

    [StringLength(100)]
    public string TransactionReference { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}
