using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

/// <summary>Used to verify or reject both loan documents and KYC documents.</summary>
public class VerifyDocumentRequest
{
    [Required, RegularExpression("^(Verified|Rejected)$", ErrorMessage = "Status must be Verified or Rejected.")]
    public string Status { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}
