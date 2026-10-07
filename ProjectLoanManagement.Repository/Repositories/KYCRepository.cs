using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class KYCRepository : RepositoryBase, IKYCRepository
{
    public KYCRepository(DbHelper db, ILogger<KYCRepository> logger) : base(db, logger)
    {
    }

    public ResultSet CreateKYC(CreateKYCRequest request, int createdBy, string ipAddress)
    {
        return Execute("KYC", "Unable to create the KYC case.", () =>
        {
            KYCVerification kyc = Db.Query("dbo.sp_CreateKYCVerification", p =>
            {
                p.AddInt("@LoanId", request.LoanId);
                p.AddVarChar("@KYCType", request.KYCType, 20);
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@CreatedBy", createdBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapKYC));

            return ResultSet.Success(kyc, "KYC case created and checklist prepared");
        });
    }

    public ResultSet GetKYC(int kycVerificationId)
    {
        return Execute("KYC", "Unable to fetch the KYC case.", () =>
        {
            KYCDetails details = Db.Query("dbo.sp_GetKYCVerification", p => p.AddInt("@KYCVerificationId", kycVerificationId), r =>
            {
                // The procedure returns four result sets in a fixed order
                KYCDetails result = new KYCDetails { Verification = r.ReadSingle(EntityMapper.MapKYC) };

                r.NextResult();
                result.Documents = r.ReadList(x => EntityMapper.MapKYCDocument(x, false));

                r.NextResult();
                result.Checklist = r.ReadList(EntityMapper.MapChecklist);

                r.NextResult();
                result.VideoSessions = r.ReadList(EntityMapper.MapVideo);

                return result;
            });

            return ResultSet.Success(details, "KYC case fetched successfully");
        });
    }

    public ResultSet GetKYCByLoan(int loanId)
    {
        return Execute("KYC", "Unable to fetch KYC cases.", () =>
        {
            List<KYCVerification> items = Db.Query("dbo.sp_GetKYCByLoan", p => p.AddInt("@LoanId", loanId),
                r => r.ReadList(EntityMapper.MapKYC));
            return ResultSet.Success(items, "KYC cases fetched successfully");
        });
    }

    public ResultSet AddKYCDocument(int kycVerificationId, int documentTypeId, string documentNumberMasked, StoredFile file, int uploadedBy, string ipAddress)
    {
        return Execute("KYC", "Unable to save the KYC document.", () =>
        {
            KYCDocument document = Db.Query("dbo.sp_AddKYCDocument", p =>
            {
                p.AddInt("@KYCVerificationId", kycVerificationId);
                p.AddInt("@DocumentTypeId", documentTypeId);
                p.AddNVarChar("@DocumentNumberMasked", documentNumberMasked, 30);
                p.AddNVarChar("@FileName", file.FileName, 255);
                p.AddNVarChar("@DocumentFilePath", file.StorageKey, 500);
                p.AddVarChar("@ContentType", file.ContentType, 100);
                p.AddInt("@UploadedBy", uploadedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(x => EntityMapper.MapKYCDocument(x, false)));

            return ResultSet.Success(document, "KYC document uploaded successfully");
        });
    }

    public ResultSet GetKYCDocumentById(int kycDocumentId)
    {
        return Execute("KYC", "Unable to fetch the KYC document.", () =>
        {
            KYCDocument document = Db.Query("dbo.sp_GetKYCDocumentById", p => p.AddInt("@KYCDocumentId", kycDocumentId),
                r => r.ReadSingle(x => EntityMapper.MapKYCDocument(x, true)));
            return ResultSet.Success(document, "KYC document fetched successfully");
        });
    }

    public ResultSet VerifyKYCDocument(int kycDocumentId, VerifyDocumentRequest request, int verifiedBy, string ipAddress)
    {
        return Execute("KYC", "Unable to update the KYC document.", () =>
        {
            KYCDocument document = Db.Query("dbo.sp_VerifyKYCDocument", p =>
            {
                p.AddInt("@KYCDocumentId", kycDocumentId);
                p.AddNVarChar("@DocumentStatus", request.Status, 20);
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@VerifiedBy", verifiedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(x => EntityMapper.MapKYCDocument(x, false)));

            return ResultSet.Success(document, "KYC document " + request.Status.ToLowerInvariant());
        });
    }

    public ResultSet GetChecklist(int kycVerificationId)
    {
        return Execute("KYC", "Unable to fetch the checklist.", () =>
        {
            List<KYCChecklist> items = Db.Query("dbo.sp_GetKYCChecklist", p => p.AddInt("@KYCVerificationId", kycVerificationId),
                r => r.ReadList(EntityMapper.MapChecklist));
            return ResultSet.Success(items, "Checklist fetched successfully");
        });
    }

    public ResultSet UpdateChecklist(int kycVerificationId, int checklistId, UpdateChecklistRequest request, int verifiedBy, string ipAddress)
    {
        return Execute("KYC", "Unable to update the checklist item.", () =>
        {
            KYCChecklist item = Db.Query("dbo.sp_UpdateKYCChecklist", p =>
            {
                p.AddInt("@KYCVerificationId", kycVerificationId);
                p.AddInt("@ChecklistId", checklistId);
                p.AddBit("@IsVerified", request.IsVerified);
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@VerifiedBy", verifiedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapChecklist));

            return ResultSet.Success(item, "Checklist item updated");
        });
    }

    public ResultSet UpdateKYCStatus(int kycVerificationId, string status, string reason, int updatedBy, string ipAddress)
    {
        return Execute("KYC", "Unable to update the KYC status.", () =>
        {
            KYCVerification kyc = Db.Query("dbo.sp_UpdateKYCStatus", p =>
            {
                p.AddInt("@KYCVerificationId", kycVerificationId);
                p.AddNVarChar("@KYCStatus", status, 20);
                p.AddNVarChar("@Reason", reason, 500);
                p.AddInt("@UpdatedBy", updatedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapKYC));

            return ResultSet.Success(kyc, "KYC status updated to " + status);
        });
    }
}
