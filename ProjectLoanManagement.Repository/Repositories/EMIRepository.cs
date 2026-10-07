using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class EMIRepository : RepositoryBase, IEMIRepository
{
    public EMIRepository(DbHelper db, ILogger<EMIRepository> logger) : base(db, logger)
    {
    }

    public ResultSet GenerateSchedule(int loanId, int userId, string ipAddress)
    {
        return Execute("EMI", "Unable to generate the EMI schedule.", () =>
        {
            Db.NonQuery("dbo.sp_GenerateEMISchedule", p =>
            {
                p.AddInt("@LoanId", loanId);
                p.AddInt("@UserId", userId);
                p.AddVarChar("@IPAddress", ipAddress, 45);
                p.AddBit("@ReturnResult", false);
            });

            // Return the full schedule so the caller sees what was created
            ResultSet schedule = GetSchedule(loanId);
            schedule.Message = "EMI schedule generated and loan is now Active";
            return schedule;
        });
    }

    public ResultSet GetSchedule(int loanId)
    {
        return Execute("EMI", "Unable to fetch the EMI schedule.", () =>
        {
            EMISchedule schedule = Db.Query("dbo.sp_GetEMISchedule", p => p.AddInt("@LoanId", loanId), r =>
            {
                EMISchedule result = r.ReadSingle(x => new EMISchedule
                {
                    LoanId = x.Field<int>("LoanId"),
                    LoanNumber = x.Field<string>("LoanNumber"),
                    LoanStatus = x.Field<string>("LoanStatus"),
                    TotalEMIs = x.Field<int>("TotalEMIs"),
                    TotalPayable = x.Field<decimal>("TotalPayable"),
                    TotalPaid = x.Field<decimal>("TotalPaid"),
                    Outstanding = x.Field<decimal>("Outstanding"),
                    NextDueDate = x.Field<DateTime?>("NextDueDate")
                });

                r.NextResult();
                result.Installments = r.ReadList(EntityMapper.MapEMI);
                return result;
            });

            return ResultSet.Success(schedule, "EMI schedule fetched successfully");
        });
    }

    public ResultSet MarkOverdue(int userId, string ipAddress)
    {
        return Execute("EMI", "Unable to mark overdue EMIs.", () =>
        {
            object updated = Db.Scalar("dbo.sp_MarkOverdueEMIs", p =>
            {
                p.AddInt("@UserId", userId);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            });

            int count = Convert.ToInt32(updated);
            return ResultSet.Success(new { UpdatedCount = count }, count + " EMI(s) marked overdue");
        });
    }
}
