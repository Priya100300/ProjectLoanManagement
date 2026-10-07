using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface IKYCRepository
{
    ResultSet CreateKYC(CreateKYCRequest request, int createdBy, string ipAddress);

    /// <summary>Data = KYCDetails (header, documents, checklist, video sessions).</summary>
    ResultSet GetKYC(int kycVerificationId);

    ResultSet GetKYCByLoan(int loanId);

    ResultSet AddKYCDocument(int kycVerificationId, int documentTypeId, string documentNumberMasked, StoredFile file, int uploadedBy, string ipAddress);

    ResultSet GetKYCDocumentById(int kycDocumentId);

    ResultSet VerifyKYCDocument(int kycDocumentId, VerifyDocumentRequest request, int verifiedBy, string ipAddress);

    ResultSet GetChecklist(int kycVerificationId);

    ResultSet UpdateChecklist(int kycVerificationId, int checklistId, UpdateChecklistRequest request, int verifiedBy, string ipAddress);

    /// <summary>status = Verified, Rejected or Rework.</summary>
    ResultSet UpdateKYCStatus(int kycVerificationId, string status, string reason, int updatedBy, string ipAddress);
}
