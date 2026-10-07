using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface IDocumentRepository
{
    ResultSet GetDocumentTypes(string appliesTo);

    ResultSet UploadLoanDocument(int loanId, int documentTypeId, StoredFile file, int uploadedBy, string ipAddress);

    ResultSet GetLoanDocuments(int loanId, bool includeInactive);

    /// <summary>Includes the storage key (FilePath) for the download endpoint.</summary>
    ResultSet GetLoanDocumentById(int documentId);

    ResultSet VerifyLoanDocument(int documentId, VerifyDocumentRequest request, int verifiedBy, string ipAddress);
}
