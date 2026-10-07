using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface IPaymentRepository
{
    ResultSet RecordPayment(RecordPaymentRequest request, int createdBy, string ipAddress);

    ResultSet GetPaymentHistory(int loanId);
}
