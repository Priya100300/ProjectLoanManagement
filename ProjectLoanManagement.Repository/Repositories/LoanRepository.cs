using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class LoanRepository : RepositoryBase, ILoanRepository
{
    public LoanRepository(DbHelper db, ILogger<LoanRepository> logger) : base(db, logger)
    {
    }

    public ResultSet CreateLoan(CreateLoanRequest request, int createdBy, string ipAddress)
    {
        return Execute("LOAN", "Unable to create the loan application.", () =>
        {
            Loan loan = Db.Query("dbo.sp_CreateLoan", p =>
            {
                p.AddInt("@CustomerId", request.CustomerId);
                p.AddNVarChar("@LoanType", request.LoanType, 30);
                p.AddDecimal("@LoanAmount", request.LoanAmount);
                p.AddDecimal("@InterestRate", request.InterestRate, 5, 2);
                p.AddInt("@TenureMonths", request.TenureMonths);
                p.AddNVarChar("@Purpose", request.Purpose, 300);
                p.AddInt("@CreatedBy", createdBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapLoan));

            return ResultSet.Success(loan, "Loan application created successfully");
        });
    }

    public ResultSet GetLoans(string loanStatus, int? customerId, int pageNumber, int pageSize)
    {
        return Execute("LOAN", "Unable to fetch loans.", () =>
        {
            PagedResult<Loan> page = Db.Query("dbo.sp_GetLoans", p =>
            {
                p.AddNVarChar("@LoanStatus", loanStatus, 20);
                p.AddInt("@CustomerId", customerId);
                p.AddInt("@CustomerUserId", null);
                p.AddInt("@PageNumber", pageNumber);
                p.AddInt("@PageSize", pageSize);
            }, r => EntityMapper.ReadPaged(r, EntityMapper.MapLoan, pageNumber, pageSize));

            return ResultSet.Success(page, "Loans fetched successfully");
        });
    }

    public ResultSet GetMyLoans(int customerUserId, int pageNumber, int pageSize)
    {
        return Execute("LOAN", "Unable to fetch your loans.", () =>
        {
            PagedResult<Loan> page = Db.Query("dbo.sp_GetLoans", p =>
            {
                p.AddNVarChar("@LoanStatus", null, 20);
                p.AddInt("@CustomerId", null);
                p.AddInt("@CustomerUserId", customerUserId);
                p.AddInt("@PageNumber", pageNumber);
                p.AddInt("@PageSize", pageSize);
            }, r => EntityMapper.ReadPaged(r, EntityMapper.MapLoan, pageNumber, pageSize));

            return ResultSet.Success(page, "Loans fetched successfully");
        });
    }

    public ResultSet GetPendingLoans()
    {
        return Execute("LOAN", "Unable to fetch pending loans.", () =>
        {
            List<Loan> loans = Db.Query("dbo.sp_GetPendingLoans", null, r => r.ReadList(EntityMapper.MapLoan));
            return ResultSet.Success(loans, "Pending loans fetched successfully");
        });
    }

    public ResultSet GetLoanById(int loanId)
    {
        return Execute("LOAN", "Unable to fetch the loan.", () =>
        {
            Loan loan = Db.Query("dbo.sp_GetLoanById", p => p.AddInt("@LoanId", loanId),
                r => r.ReadSingle(EntityMapper.MapLoan));
            return ResultSet.Success(loan, "Loan fetched successfully");
        });
    }

    public ResultSet GetLoanStatus(int loanId)
    {
        return Execute("LOAN", "Unable to fetch the loan status.", () =>
        {
            LoanStatusInfo status = Db.Query("dbo.sp_GetLoanStatus", p => p.AddInt("@LoanId", loanId),
                r => r.ReadSingle(EntityMapper.MapLoanStatus));
            return ResultSet.Success(status, "Loan status fetched successfully");
        });
    }

    public ResultSet UpdateLoan(int loanId, UpdateLoanRequest request, int updatedBy, string ipAddress)
    {
        return Execute("LOAN", "Unable to update the loan.", () =>
        {
            Loan loan = Db.Query("dbo.sp_UpdateLoan", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddNVarChar("@LoanType", request.LoanType, 30);
                p.AddDecimal("@LoanAmount", request.LoanAmount);
                p.AddDecimal("@InterestRate", request.InterestRate, 5, 2);
                p.AddInt("@TenureMonths", request.TenureMonths);
                p.AddNVarChar("@Purpose", request.Purpose, 300);
                p.AddInt("@UpdatedBy", updatedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapLoan));

            return ResultSet.Success(loan, "Loan updated successfully");
        });
    }

    public ResultSet ApproveLoan(int loanId, ApproveLoanRequest request, int approvedBy, string ipAddress)
    {
        return Execute("LOAN", "Unable to approve the loan.", () =>
        {
            Loan loan = Db.Query("dbo.sp_ApproveLoan", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddDecimal("@ApprovedAmount", request?.ApprovedAmount);
                p.AddNVarChar("@Remarks", request?.Remarks, 500);
                p.AddInt("@ApprovedBy", approvedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapLoan));

            return ResultSet.Success(loan, "Loan approved successfully");
        });
    }

    public ResultSet RejectLoan(int loanId, string reason, int rejectedBy, string ipAddress)
    {
        return Execute("LOAN", "Unable to reject the loan.", () =>
        {
            Loan loan = Db.Query("dbo.sp_RejectLoan", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddNVarChar("@Reason", reason, 500);
                p.AddInt("@RejectedBy", rejectedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapLoan));

            return ResultSet.Success(loan, "Loan rejected");
        });
    }

    public ResultSet DisburseLoan(int loanId, DisburseLoanRequest request, string beneficiaryAccountMasked, int disbursedBy, string ipAddress)
    {
        return Execute("LOAN", "Unable to disburse the loan.", () =>
        {
            Loan loan = Db.Query("dbo.sp_DisburseLoan", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddVarChar("@DisbursementMode", request.DisbursementMode, 20);
                p.AddVarChar("@BeneficiaryAccountMasked", beneficiaryAccountMasked, 20);
                p.AddChar("@IFSCCode", request.IFSCCode, 11);
                p.AddNVarChar("@TransactionReference", request.TransactionReference, 100);
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@DisbursedBy", disbursedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapLoan));

            return ResultSet.Success(loan, "Loan disbursed, EMI schedule generated and loan is now Active");
        });
    }

    public ResultSet CloseLoan(int loanId, int closedBy, string ipAddress)
    {
        return Execute("LOAN", "Unable to close the loan.", () =>
        {
            Loan loan = Db.Query("dbo.sp_CloseLoan", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddInt("@ClosedBy", closedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapLoan));

            return ResultSet.Success(loan, "Loan closed successfully");
        });
    }

    public ResultSet CheckLoanAccess(int loanId, int userId)
    {
        return Execute("LOAN", "Unable to check access to the loan.", () =>
        {
            object hasAccess = Db.Scalar("dbo.sp_CheckLoanAccess", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddInt("@UserId", userId);
            });

            return hasAccess is bool allowed && allowed
                ? ResultSet.Success(null, "Access granted")
                : ResultSet.Failure("You do not have access to this loan.", "LOAN_403");
        });
    }
}
