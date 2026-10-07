using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class BusinessRuleRepository : RepositoryBase, IBusinessRuleRepository
{
    public BusinessRuleRepository(DbHelper db, ILogger<BusinessRuleRepository> logger) : base(db, logger)
    {
    }

    public ResultSet GetRules()
    {
        return Execute("RULE", "Unable to fetch business rules.", () =>
        {
            List<BusinessRule> rules = Db.Query("dbo.sp_GetBusinessRules", null, r => r.ReadList(EntityMapper.MapRule));
            return ResultSet.Success(rules, "Business rules fetched successfully");
        });
    }

    public ResultSet UpdateRule(string ruleKey, decimal ruleValue, int updatedBy, string ipAddress)
    {
        return Execute("RULE", "Unable to update the business rule.", () =>
        {
            Db.NonQuery("dbo.sp_UpdateBusinessRule", p =>
            {
                p.AddVarChar("@RuleKey", ruleKey, 50);
                p.AddDecimal("@RuleValue", ruleValue);
                p.AddInt("@UpdatedBy", updatedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            });

            return ResultSet.Success(null, "Business rule " + ruleKey + " updated");
        });
    }
}
