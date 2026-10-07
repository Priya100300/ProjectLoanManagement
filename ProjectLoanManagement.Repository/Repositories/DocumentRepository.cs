using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class DocumentRepository : RepositoryBase, IDocumentRepository
{
    public DocumentRepository(DbHelper db, ILogger<DocumentRepository> logger) : base(db, logger)
    {
    }

    public ResultSet GetDocumentTypes(string appliesTo)
    {
        return Execute("DOC", "Unable to fetch document types.", () =>
        {
            List<DocumentType> types = Db.Query("dbo.sp_GetDocumentTypes", p => p.AddVarChar("@AppliesTo", appliesTo, 10),
                r => r.ReadList(EntityMapper.MapDocumentType));
            return ResultSet.Success(types, "Document types fetched successfully");
        });
    }

    public ResultSet UploadLoanDocument(int loanId, int documentTypeId, StoredFile file, int uploadedBy, string ipAddress)
    {
        return Execute("DOC", "Unable to save the document.", () =>
        {
            LoanDocument document = Db.Query("dbo.sp_UploadLoanDocument", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddInt("@DocumentTypeId", documentTypeId);
                p.AddNVarChar("@FileName", file.FileName, 255);
                p.AddNVarChar("@FilePath", file.StorageKey, 500);
                p.AddVarChar("@ContentType", file.ContentType, 100);
                p.AddBigInt("@FileSizeBytes", file.SizeBytes);
                p.AddInt("@UploadedBy", uploadedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(x => EntityMapper.MapLoanDocument(x, false)));

            return ResultSet.Success(document, "Document uploaded successfully");
        });
    }

    public ResultSet GetLoanDocuments(int loanId, bool includeInactive)
    {
        return Execute("DOC", "Unable to fetch documents.", () =>
        {
            List<LoanDocument> documents = Db.Query("dbo.sp_GetLoanDocuments", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddBit("@IncludeInactive", includeInactive);
            }, r => r.ReadList(x => EntityMapper.MapLoanDocument(x, false)));

            return ResultSet.Success(documents, "Documents fetched successfully");
        });
    }

    public ResultSet GetLoanDocumentById(int documentId)
    {
        return Execute("DOC", "Unable to fetch the document.", () =>
        {
            LoanDocument document = Db.Query("dbo.sp_GetLoanDocumentById", p => p.AddInt("@DocumentId", documentId),
                r => r.ReadSingle(x => EntityMapper.MapLoanDocument(x, true)));
            return ResultSet.Success(document, "Document fetched successfully");
        });
    }

    public ResultSet VerifyLoanDocument(int documentId, VerifyDocumentRequest request, int verifiedBy, string ipAddress)
    {
        return Execute("DOC", "Unable to update the document status.", () =>
        {
            LoanDocument document = Db.Query("dbo.sp_VerifyLoanDocument", p =>
            {
                p.AddInt("@DocumentId", documentId);
                p.AddNVarChar("@VerificationStatus", request.Status, 20);
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@VerifiedBy", verifiedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(x => EntityMapper.MapLoanDocument(x, false)));

            return ResultSet.Success(document, "Document " + request.Status.ToLowerInvariant());
        });
    }
}
