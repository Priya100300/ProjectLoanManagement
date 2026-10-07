using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>Configurable policy values (FOIR limit, minimum income, lockout threshold...).</summary>
[Route("api/business-rule")]
[Authorize(Roles = Roles.Staff)]
public class BusinessRuleController : BaseApiController
{
    private readonly IBusinessRuleRepository _ruleRepository;

    public BusinessRuleController(IBusinessRuleRepository ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }

    [HttpGet]
    public IActionResult GetRules()
    {
        return FromResult(_ruleRepository.GetRules());
    }

    [HttpPut("{ruleKey}")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult UpdateRule(string ruleKey, [FromBody] UpdateBusinessRuleRequest request)
    {
        return FromResult(_ruleRepository.UpdateRule(ruleKey, request.RuleValue.Value, CurrentUserId, ClientIp));
    }
}
