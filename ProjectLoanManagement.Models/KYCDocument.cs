using System.Text.Json.Serialization;

namespace ProjectLoanManagement.Models;

public class KYCDocument
{
    public int KYCDocumentId { get; set; }
    public int KYCVerificationId { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentTypeCode { get; set; }
    public string DocumentTypeName { get; set; }
    public string DocumentNumberMasked { get; set; }
    public string DocumentStatus { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public DateTime? VerifiedDate { get; set; }
    public int? VerifiedBy { get; set; }
    public string Remarks { get; set; }
    public int UploadedBy { get; set; }
    public DateTime CreatedDate { get; set; }

    [JsonIgnore]
    public string DocumentFilePath { get; set; }
}
