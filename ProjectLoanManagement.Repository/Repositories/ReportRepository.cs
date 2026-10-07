using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

/// <summary>
/// Reports return several result sets. Each is read as a list of rows and given a name,
/// e.g. { "byStatus": [...], "byType": [...] }.
/// </summary>
public class ReportRepository : RepositoryBase, IReportRepository
{
    public ReportRepository(DbHelper db, ILogger<ReportRepository> logger) : base(db, logger)
    {
    }

    public ResultSet GetLoanReport(DateTime? fromDate, DateTime? toDate)
    {
        return RunReport("dbo.sp_GetLoanReport", fromDate, toDate, "Loan report", "byStatus", "byType");
    }

    public ResultSet GetKYCReport(DateTime? fromDate, DateTime? toDate)
    {
        return RunReport("dbo.sp_GetKYCReport", fromDate, toDate, "KYC report", "byStatus", "turnaround", "videoSessions");
    }

    public ResultSet GetPaymentReport(DateTime? fromDate, DateTime? toDate)
    {
        return RunReport("dbo.sp_GetPaymentReport", fromDate, toDate, "Payment report", "byMode", "overdue");
    }

    private ResultSet RunReport(string procedureName, DateTime? fromDate, DateTime? toDate, string title, params string[] sectionNames)
    {
        return Execute("REPORT", "Unable to generate the " + title.ToLowerInvariant() + ".", () =>
        {
            Dictionary<string, object> report = Db.Query(procedureName, p =>
            {
                p.AddDate("@FromDate", fromDate);
                p.AddDate("@ToDate", toDate);
            }, r => ReadSections(r, sectionNames));

            DateTime to = (toDate ?? DateTime.UtcNow).Date;
            report["period"] = new { From = (fromDate ?? to.AddDays(-30)).Date, To = to };
            return ResultSet.Success(report, title + " generated successfully");
        });
    }

    private static Dictionary<string, object> ReadSections(SqlDataReader reader, string[] sectionNames)
    {
        Dictionary<string, object> sections = new Dictionary<string, object>();
        for (int i = 0; i < sectionNames.Length; i++)
        {
            if (i > 0 && !reader.NextResult())
            {
                break;
            }

            sections[sectionNames[i]] = reader.ReadRows();
        }

        return sections;
    }
}
