using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.Interfaces;

public interface IReportRepository
{
    ResultSet GetLoanReport(DateTime? fromDate, DateTime? toDate);

    ResultSet GetKYCReport(DateTime? fromDate, DateTime? toDate);

    ResultSet GetPaymentReport(DateTime? fromDate, DateTime? toDate);
}
