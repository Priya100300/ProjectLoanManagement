using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.Interfaces;

public interface IAuditRepository
{
    ResultSet WriteLog(int? userId, string action, string module, string referenceId, string description, string ipAddress);

    ResultSet GetAuditLogs(string module, string referenceId, int? userId, DateTime? fromDate, DateTime? toDate,
                           int pageNumber, int pageSize);
}
