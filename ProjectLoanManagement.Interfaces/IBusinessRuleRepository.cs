using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.Interfaces;

public interface IBusinessRuleRepository
{
    ResultSet GetRules();

    ResultSet UpdateRule(string ruleKey, decimal ruleValue, int updatedBy, string ipAddress);
}
