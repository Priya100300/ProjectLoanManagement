using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class AuditRepository : RepositoryBase, IAuditRepository
{
    public AuditRepository(DbHelper db, ILogger<AuditRepository> logger) : base(db, logger)
    {
    }

    public ResultSet WriteLog(int? userId, string action, string module, string referenceId, string description, string ipAddress)
    {
        return Execute("AUDIT", "Unable to write the audit log.", () =>
        {
            Db.NonQuery("dbo.sp_WriteAuditLog", p =>
            {
                p.AddInt("@UserId", userId);
                p.AddVarChar("@Action", action, 50);
                p.AddVarChar("@Module", module, 50);
                p.AddNVarChar("@ReferenceId", referenceId, 50);
                p.AddNVarChar("@Description", description, 1000);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            });

            return ResultSet.Success(null, "Audit log written");
        });
    }

    public ResultSet GetAuditLogs(string module, string referenceId, int? userId, DateTime? fromDate, DateTime? toDate,
                                  int pageNumber, int pageSize)
    {
        return Execute("AUDIT", "Unable to fetch audit logs.", () =>
        {
            PagedResult<AuditLog> page = Db.Query("dbo.sp_GetAuditLogs", p =>
            {
                p.AddVarChar("@Module", module, 50);
                p.AddNVarChar("@ReferenceId", referenceId, 50);
                p.AddInt("@UserId", userId);
                p.AddDate("@FromDate", fromDate);
                p.AddDate("@ToDate", toDate);
                p.AddInt("@PageNumber", pageNumber);
                p.AddInt("@PageSize", pageSize);
            }, r => EntityMapper.ReadPaged(r, EntityMapper.MapAudit, pageNumber, pageSize));

            return ResultSet.Success(page, "Audit logs fetched successfully");
        });
    }
}
