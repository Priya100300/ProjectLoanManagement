using System.Text.Json.Serialization;

namespace ProjectLoanManagement.Models;

public class DocumentType
{
    public int DocumentTypeId { get; set; }
    public string DocumentTypeCode { get; set; }
    public string DocumentTypeName { get; set; }
    public string AppliesTo { get; set; }
    public bool IsMandatoryForKYC { get; set; }
    public bool IsMandatoryForLoan { get; set; }
    public int DisplayOrder { get; set; }
}

public class LoanDocument
{
    public int DocumentId { get; set; }
    public int LoanId { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentTypeCode { get; set; }
    public string DocumentTypeName { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string VerificationStatus { get; set; }
    public string Remarks { get; set; }
    public bool IsActive { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedDate { get; set; }
    public int? VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }

    /// <summary>Storage key. Used internally for downloads, never sent to the client.</summary>
    [JsonIgnore]
    public string FilePath { get; set; }
}
