using Microsoft.Data.SqlClient;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.Repository.Data;

/// <summary>
/// Converts a SqlDataReader row into a model. Column names match the views / SELECT lists
/// in 02_StoredProcedures.sql, so each entity is mapped in exactly one place.
/// </summary>
internal static class EntityMapper
{
    public static User MapUser(SqlDataReader r)
    {
        return new User
        {
            UserId = r.Field<int>("UserId"),
            UserName = r.Field<string>("UserName"),
            FullName = r.Field<string>("FullName"),
            Email = r.Field<string>("Email"),
            Role = r.Field<string>("Role"),
            IsActive = r.Field<bool>("IsActive"),
            FailedLoginCount = r.Field<int>("FailedLoginCount"),
            LastLoginDate = r.Field<DateTime?>("LastLoginDate"),
            CreatedDate = r.Field<DateTime>("CreatedDate")
        };
    }

    public static Customer MapCustomer(SqlDataReader r)
    {
        return new Customer
        {
            CustomerId = r.Field<int>("CustomerId"),
            CustomerCode = r.Field<string>("CustomerCode"),
            UserId = r.Field<int?>("UserId"),
            FullName = r.Field<string>("FullName"),
            MobileNo = r.Field<string>("MobileNo"),
            Email = r.Field<string>("Email"),
            Address = r.Field<string>("Address"),
            City = r.Field<string>("City"),
            State = r.Field<string>("State"),
            PinCode = r.Field<string>("PinCode"),
            PANMasked = r.Field<string>("PANMasked"),
            AadhaarNoMasked = r.Field<string>("AadhaarNoMasked"),
            DateOfBirth = r.Field<DateTime>("DateOfBirth"),
            IsActive = r.Field<bool>("IsActive"),
            CreatedDate = r.Field<DateTime>("CreatedDate"),
            UpdatedDate = r.Field<DateTime?>("UpdatedDate")
        };
    }

    public static Loan MapLoan(SqlDataReader r)
    {
        return new Loan
        {
            LoanId = r.Field<int>("LoanId"),
            LoanNumber = r.Field<string>("LoanNumber"),
            CustomerId = r.Field<int>("CustomerId"),
            CustomerCode = r.Field<string>("CustomerCode"),
            CustomerName = r.Field<string>("CustomerName"),
            LoanType = r.Field<string>("LoanType"),
            LoanAmount = r.Field<decimal>("LoanAmount"),
            ApprovedAmount = r.Field<decimal?>("ApprovedAmount"),
            InterestRate = r.Field<decimal>("InterestRate"),
            TenureMonths = r.Field<int>("TenureMonths"),
            Purpose = r.Field<string>("Purpose"),
            LoanStatus = r.Field<string>("LoanStatus"),
            ApplicationDate = r.Field<DateTime>("ApplicationDate"),
            ApprovalDate = r.Field<DateTime?>("ApprovalDate"),
            ApprovedBy = r.Field<int?>("ApprovedBy"),
            ApprovedByName = r.Field<string>("ApprovedByName"),
            RejectionDate = r.Field<DateTime?>("RejectionDate"),
            RejectionReason = r.Field<string>("RejectionReason"),
            DisbursementDate = r.Field<DateTime?>("DisbursementDate"),
            DisbursedAmount = r.Field<decimal?>("DisbursedAmount"),
            DisbursementMode = r.Field<string>("DisbursementMode"),
            ClosedDate = r.Field<DateTime?>("ClosedDate"),
            KYCVerificationId = r.Field<int?>("KYCVerificationId"),
            KYCStatus = r.Field<string>("KYCStatus"),
            AssessmentId = r.Field<int?>("AssessmentId"),
            AssessmentStatus = r.Field<string>("AssessmentStatus"),
            EligibleAmount = r.Field<decimal?>("EligibleAmount"),
            CreatedBy = r.Field<int>("CreatedBy"),
            CreatedDate = r.Field<DateTime>("CreatedDate")
        };
    }

    public static LoanStatusInfo MapLoanStatus(SqlDataReader r)
    {
        return new LoanStatusInfo
        {
            LoanId = r.Field<int>("LoanId"),
            LoanNumber = r.Field<string>("LoanNumber"),
            LoanStatus = r.Field<string>("LoanStatus"),
            KYCStatus = r.Field<string>("KYCStatus"),
            AssessmentStatus = r.Field<string>("AssessmentStatus"),
            TotalPayable = r.Field<decimal>("TotalPayable"),
            TotalPaid = r.Field<decimal>("TotalPaid"),
            Outstanding = r.Field<decimal>("Outstanding"),
            NextDueDate = r.Field<DateTime?>("NextDueDate")
        };
    }

    public static LoanAssessment MapAssessment(SqlDataReader r)
    {
        return new LoanAssessment
        {
            AssessmentId = r.Field<int>("AssessmentId"),
            LoanId = r.Field<int>("LoanId"),
            MonthlyIncome = r.Field<decimal>("MonthlyIncome"),
            ExistingMonthlyObligations = r.Field<decimal>("ExistingMonthlyObligations"),
            IsIncomeVerified = r.Field<bool>("IsIncomeVerified"),
            ProposedEMI = r.Field<decimal?>("ProposedEMI"),
            FOIRPercent = r.Field<decimal?>("FOIRPercent"),
            EligibleAmount = r.Field<decimal?>("EligibleAmount"),
            RiskCategory = r.Field<string>("RiskCategory"),
            AssessmentStatus = r.Field<string>("AssessmentStatus"),
            Remarks = r.Field<string>("Remarks"),
            AssessedBy = r.Field<int>("AssessedBy"),
            AssessedByName = r.Field<string>("AssessedByName"),
            AssessedDate = r.Field<DateTime?>("AssessedDate"),
            CreatedDate = r.Field<DateTime>("CreatedDate")
        };
    }

    public static DocumentType MapDocumentType(SqlDataReader r)
    {
        return new DocumentType
        {
            DocumentTypeId = r.Field<int>("DocumentTypeId"),
            DocumentTypeCode = r.Field<string>("DocumentTypeCode"),
            DocumentTypeName = r.Field<string>("DocumentTypeName"),
            AppliesTo = r.Field<string>("AppliesTo"),
            IsMandatoryForKYC = r.Field<bool>("IsMandatoryForKYC"),
            IsMandatoryForLoan = r.Field<bool>("IsMandatoryForLoan"),
            DisplayOrder = r.Field<int>("DisplayOrder")
        };
    }

    public static LoanDocument MapLoanDocument(SqlDataReader r, bool includePath)
    {
        return new LoanDocument
        {
            DocumentId = r.Field<int>("DocumentId"),
            LoanId = r.Field<int>("LoanId"),
            DocumentTypeId = r.Field<int>("DocumentTypeId"),
            DocumentTypeCode = r.Field<string>("DocumentTypeCode"),
            DocumentTypeName = r.Field<string>("DocumentTypeName"),
            FileName = r.Field<string>("FileName"),
            ContentType = r.Field<string>("ContentType"),
            FileSizeBytes = r.Field<long>("FileSizeBytes"),
            VerificationStatus = r.Field<string>("VerificationStatus"),
            Remarks = r.Field<string>("Remarks"),
            IsActive = r.Field<bool>("IsActive"),
            UploadedBy = r.Field<int>("UploadedBy"),
            UploadedDate = r.Field<DateTime>("UploadedDate"),
            VerifiedBy = r.Field<int?>("VerifiedBy"),
            VerifiedDate = r.Field<DateTime?>("VerifiedDate"),
            FilePath = includePath ? r.Field<string>("FilePath") : null
        };
    }

    public static KYCVerification MapKYC(SqlDataReader r)
    {
        return new KYCVerification
        {
            KYCVerificationId = r.Field<int>("KYCVerificationId"),
            CustomerId = r.Field<int>("CustomerId"),
            CustomerName = r.Field<string>("CustomerName"),
            LoanId = r.Field<int?>("LoanId"),
            LoanNumber = r.Field<string>("LoanNumber"),
            KYCType = r.Field<string>("KYCType"),
            KYCStatus = r.Field<string>("KYCStatus"),
            VerificationDate = r.Field<DateTime?>("VerificationDate"),
            VerifiedBy = r.Field<int?>("VerifiedBy"),
            VerifiedByName = r.Field<string>("VerifiedByName"),
            RejectionReason = r.Field<string>("RejectionReason"),
            Remarks = r.Field<string>("Remarks"),
            CreatedDate = r.Field<DateTime>("CreatedDate")
        };
    }

    public static KYCDocument MapKYCDocument(SqlDataReader r, bool includePath)
    {
        return new KYCDocument
        {
            KYCDocumentId = r.Field<int>("KYCDocumentId"),
            KYCVerificationId = r.Field<int>("KYCVerificationId"),
            DocumentTypeId = r.Field<int>("DocumentTypeId"),
            DocumentTypeCode = r.Field<string>("DocumentTypeCode"),
            DocumentTypeName = r.Field<string>("DocumentTypeName"),
            DocumentNumberMasked = r.Field<string>("DocumentNumberMasked"),
            DocumentStatus = r.Field<string>("DocumentStatus"),
            FileName = r.Field<string>("FileName"),
            ContentType = r.Field<string>("ContentType"),
            VerifiedDate = r.Field<DateTime?>("VerifiedDate"),
            VerifiedBy = r.Field<int?>("VerifiedBy"),
            Remarks = r.Field<string>("Remarks"),
            UploadedBy = r.Field<int>("UploadedBy"),
            CreatedDate = r.Field<DateTime>("CreatedDate"),
            DocumentFilePath = includePath ? r.Field<string>("DocumentFilePath") : null
        };
    }

    public static KYCChecklist MapChecklist(SqlDataReader r)
    {
        return new KYCChecklist
        {
            ChecklistId = r.Field<int>("ChecklistId"),
            KYCVerificationId = r.Field<int>("KYCVerificationId"),
            ChecklistMasterId = r.Field<int>("ChecklistMasterId"),
            ChecklistItem = r.Field<string>("ChecklistItem"),
            IsMandatory = r.Field<bool>("IsMandatory"),
            IsVerified = r.Field<bool>("IsVerified"),
            Remarks = r.Field<string>("Remarks"),
            VerifiedBy = r.Field<int?>("VerifiedBy"),
            VerifiedDate = r.Field<DateTime?>("VerifiedDate")
        };
    }

    public static VideoVerification MapVideo(SqlDataReader r)
    {
        return new VideoVerification
        {
            VerificationId = r.Field<int>("VerificationId"),
            KYCVerificationId = r.Field<int>("KYCVerificationId"),
            CustomerId = r.Field<int>("CustomerId"),
            CustomerName = r.Field<string>("CustomerName"),
            LoanId = r.Field<int?>("LoanId"),
            OfficerId = r.Field<int>("OfficerId"),
            OfficerName = r.Field<string>("OfficerName"),
            ScheduledDate = r.Field<DateTime>("ScheduledDate"),
            StartTime = r.Field<DateTime?>("StartTime"),
            EndTime = r.Field<DateTime?>("EndTime"),
            VerificationStatus = r.Field<string>("VerificationStatus"),
            ExternalSessionId = r.Field<string>("ExternalSessionId"),
            Remarks = r.Field<string>("Remarks"),
            CreatedDate = r.Field<DateTime>("CreatedDate")
        };
    }

    public static VideoRecording MapRecording(SqlDataReader r, bool includePath)
    {
        return new VideoRecording
        {
            RecordingId = r.Field<int>("RecordingId"),
            VerificationId = r.Field<int>("VerificationId"),
            FileName = r.Field<string>("FileName"),
            ContentType = r.Field<string>("ContentType"),
            FileSize = r.Field<long>("FileSize"),
            DurationSeconds = r.Field<int>("DurationSeconds"),
            FileHashSha256 = r.Field<string>("FileHashSha256"),
            RecordingStartTime = r.Field<DateTime>("RecordingStartTime"),
            RecordingEndTime = r.Field<DateTime>("RecordingEndTime"),
            UploadedBy = r.Field<int>("UploadedBy"),
            UploadedDate = r.Field<DateTime>("UploadedDate"),
            FilePath = includePath ? r.Field<string>("FilePath") : null
        };
    }

    public static LoanEMI MapEMI(SqlDataReader r)
    {
        return new LoanEMI
        {
            EMIId = r.Field<int>("EMIId"),
            LoanId = r.Field<int>("LoanId"),
            EMINumber = r.Field<int>("EMINumber"),
            DueDate = r.Field<DateTime>("DueDate"),
            OpeningBalance = r.Field<decimal>("OpeningBalance"),
            EMIAmount = r.Field<decimal>("EMIAmount"),
            PrincipalAmount = r.Field<decimal>("PrincipalAmount"),
            InterestAmount = r.Field<decimal>("InterestAmount"),
            ClosingBalance = r.Field<decimal>("ClosingBalance"),
            PaidAmount = r.Field<decimal>("PaidAmount"),
            EMIStatus = r.Field<string>("EMIStatus"),
            PaidDate = r.Field<DateTime?>("PaidDate")
        };
    }

    public static LoanPayment MapPayment(SqlDataReader r)
    {
        return new LoanPayment
        {
            PaymentId = r.Field<int>("PaymentId"),
            LoanId = r.Field<int>("LoanId"),
            EMIId = r.Field<int?>("EMIId"),
            EMINumber = r.Field<int?>("EMINumber"),
            EMIStatus = r.Field<string>("EMIStatus"),
            PaymentReference = r.Field<string>("PaymentReference"),
            PaymentDate = r.Field<DateTime>("PaymentDate"),
            Amount = r.Field<decimal>("Amount"),
            PaymentMode = r.Field<string>("PaymentMode"),
            ExternalTransactionId = r.Field<string>("ExternalTransactionId"),
            Remarks = r.Field<string>("Remarks"),
            CreatedBy = r.Field<int>("CreatedBy"),
            CreatedDate = r.Field<DateTime>("CreatedDate")
        };
    }

    public static AuditLog MapAudit(SqlDataReader r)
    {
        return new AuditLog
        {
            AuditId = r.Field<long>("AuditId"),
            UserId = r.Field<int?>("UserId"),
            UserName = r.Field<string>("UserName"),
            Action = r.Field<string>("Action"),
            Module = r.Field<string>("Module"),
            ReferenceId = r.Field<string>("ReferenceId"),
            Description = r.Field<string>("Description"),
            IPAddress = r.Field<string>("IPAddress"),
            CreatedDate = r.Field<DateTime>("CreatedDate")
        };
    }

    public static BusinessRule MapRule(SqlDataReader r)
    {
        return new BusinessRule
        {
            RuleKey = r.Field<string>("RuleKey"),
            RuleValue = r.Field<decimal>("RuleValue"),
            Description = r.Field<string>("Description"),
            UpdatedDate = r.Field<DateTime>("UpdatedDate"),
            UpdatedBy = r.Field<int?>("UpdatedBy")
        };
    }

    /// <summary>Reads a paged list whose rows carry a TotalCount column (COUNT(*) OVER ()).</summary>
    public static PagedResult<T> ReadPaged<T>(SqlDataReader r, Func<SqlDataReader, T> map, int pageNumber, int pageSize)
    {
        PagedResult<T> page = new PagedResult<T> { PageNumber = pageNumber, PageSize = pageSize };
        while (r.Read())
        {
            page.TotalCount = r.Field<int>("TotalCount");
            page.Items.Add(map(r));
        }

        return page;
    }
}
