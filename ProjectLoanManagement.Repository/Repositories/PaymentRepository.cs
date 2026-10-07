using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class PaymentRepository : RepositoryBase, IPaymentRepository
{
    public PaymentRepository(DbHelper db, ILogger<PaymentRepository> logger) : base(db, logger)
    {
    }

    public ResultSet RecordPayment(RecordPaymentRequest request, int createdBy, string ipAddress)
    {
        return Execute("PAY", "Unable to record the payment.", () =>
        {
            LoanPayment payment = Db.Query("dbo.sp_RecordPayment", p =>
            {
                p.AddInt("@LoanId", request.LoanId);
                p.AddInt("@EMIId", request.EMIId);
                p.AddNVarChar("@PaymentReference", request.PaymentReference, 50);
                p.AddDecimal("@Amount", request.Amount);
                p.AddNVarChar("@PaymentMode", request.PaymentMode, 20);
                p.AddNVarChar("@ExternalTransactionId", request.ExternalTransactionId, 100);
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@CreatedBy", createdBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapPayment));

            return ResultSet.Success(payment, "Payment recorded successfully");
        });
    }

    public ResultSet GetPaymentHistory(int loanId)
    {
        return Execute("PAY", "Unable to fetch the payment history.", () =>
        {
            List<LoanPayment> payments = Db.Query("dbo.sp_GetPaymentHistory", p => p.AddInt("@LoanId", loanId),
                r => r.ReadList(EntityMapper.MapPayment));
            return ResultSet.Success(payments, "Payment history fetched successfully");
        });
    }
}
