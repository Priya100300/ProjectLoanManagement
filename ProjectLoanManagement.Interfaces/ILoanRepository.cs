using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface ILoanRepository
{
    ResultSet CreateLoan(CreateLoanRequest request, int createdBy, string ipAddress);

    ResultSet GetLoans(string loanStatus, int? customerId, int pageNumber, int pageSize);

    ResultSet GetMyLoans(int customerUserId, int pageNumber, int pageSize);

    ResultSet GetPendingLoans();

    ResultSet GetLoanById(int loanId);

    ResultSet GetLoanStatus(int loanId);

    ResultSet UpdateLoan(int loanId, UpdateLoanRequest request, int updatedBy, string ipAddress);

    ResultSet ApproveLoan(int loanId, ApproveLoanRequest request, int approvedBy, string ipAddress);

    ResultSet RejectLoan(int loanId, string reason, int rejectedBy, string ipAddress);

    ResultSet DisburseLoan(int loanId, DisburseLoanRequest request, string beneficiaryAccountMasked, int disbursedBy, string ipAddress);

    ResultSet CloseLoan(int loanId, int closedBy, string ipAddress);

    /// <summary>Success when the loan belongs to the customer linked to this login; LOAN_403 otherwise.</summary>
    ResultSet CheckLoanAccess(int loanId, int userId);
}
