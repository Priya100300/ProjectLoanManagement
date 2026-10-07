using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Controllers;

[Route("api/emi")]
[Authorize]
public class EMIController : BaseApiController
{
    private readonly IEMIRepository _emiRepository;
    private readonly ILoanRepository _loanRepository;

    public EMIController(IEMIRepository emiRepository, ILoanRepository loanRepository)
    {
        _emiRepository = emiRepository;
        _loanRepository = loanRepository;
    }

    /// <summary>Summary plus every instalment (principal / interest split, paid amount, status).</summary>
    [HttpGet("loan/{loanId:int}")]
    [Authorize(Roles = Roles.StaffAndCustomer)]
    public IActionResult GetSchedule(int loanId)
    {
        IActionResult denied = DenyIfNotOwnLoan(_loanRepository, loanId);
        return denied ?? FromResult(_emiRepository.GetSchedule(loanId));
    }

    /// <summary>
    /// Normally not needed: disbursement already generates the schedule. Use only for a loan
    /// left in Disbursed status without a schedule.
    /// </summary>
    [HttpPost("generate/{loanId:int}")]
    [Authorize(Roles = Roles.Approvers)]
    public IActionResult Generate(int loanId)
    {
        return FromResult(_emiRepository.GenerateSchedule(loanId, CurrentUserId, ClientIp));
    }

    /// <summary>Marks unpaid EMIs past their due date as Overdue. Schedule as a daily job in production.</summary>
    [HttpPost("mark-overdue")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult MarkOverdue()
    {
        return FromResult(_emiRepository.MarkOverdue(CurrentUserId, ClientIp));
    }
}
