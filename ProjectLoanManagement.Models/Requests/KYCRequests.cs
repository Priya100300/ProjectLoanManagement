using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

public class CreateKYCRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? LoanId { get; set; }

    /// <summary>Document = documents only, Video = video only, Full = both.</summary>
    [Required, RegularExpression("^(Document|Video|Full)$", ErrorMessage = "KYCType must be Document, Video or Full.")]
    public string KYCType { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}

public class KYCDecisionRequest
{
    /// <summary>Required for reject and rework, optional for verify.</summary>
    [StringLength(500)]
    public string Reason { get; set; }
}

public class UpdateChecklistRequest
{
    [Required]
    public bool? IsVerified { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}
