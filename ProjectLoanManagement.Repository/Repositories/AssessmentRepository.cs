using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class AssessmentRepository : RepositoryBase, IAssessmentRepository
{
    public AssessmentRepository(DbHelper db, ILogger<AssessmentRepository> logger) : base(db, logger)
    {
    }

    public ResultSet CreateAssessment(CreateAssessmentRequest request, int assessedBy, string ipAddress)
    {
        return Execute("ASSESS", "Unable to save the assessment.", () =>
        {
            LoanAssessment assessment = Db.Query("dbo.sp_CreateLoanAssessment", p =>
            {
                p.AddInt("@LoanId", request.LoanId);
                p.AddDecimal("@MonthlyIncome", request.MonthlyIncome);
                p.AddDecimal("@ExistingMonthlyObligations", request.ExistingMonthlyObligations);
                p.AddBit("@IsIncomeVerified", request.IsIncomeVerified);
                p.AddVarChar("@RiskCategory", request.RiskCategory, 10);
                p.AddNVarChar("@AssessmentStatus", request.AssessmentStatus, 20);
                p.AddNVarChar("@Remarks", request.Remarks, 1000);
                p.AddInt("@AssessedBy", assessedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapAssessment));

            return ResultSet.Success(assessment, "Assessment saved successfully");
        });
    }

    public ResultSet GetAssessments(int loanId)
    {
        return Execute("ASSESS", "Unable to fetch assessments.", () =>
        {
            List<LoanAssessment> items = Db.Query("dbo.sp_GetLoanAssessments", p => p.AddInt("@LoanId", loanId),
                r => r.ReadList(EntityMapper.MapAssessment));
            return ResultSet.Success(items, "Assessments fetched successfully");
        });
    }
}
