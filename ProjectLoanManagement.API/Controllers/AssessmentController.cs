using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>
/// Credit assessment. The database calculates proposed EMI, FOIR and eligible amount from the
/// configured business rules; the officer records the decision.
/// </summary>
[Route("api/assessment")]
[Authorize(Roles = Roles.Staff)]
public class AssessmentController : BaseApiController
{
    private readonly IAssessmentRepository _assessmentRepository;

    public AssessmentController(IAssessmentRepository assessmentRepository)
    {
        _assessmentRepository = assessmentRepository;
    }

    [HttpPost]
    [Authorize(Roles = Roles.LoanDesk)]
    public IActionResult CreateAssessment([FromBody] CreateAssessmentRequest request)
    {
        return FromResult(_assessmentRepository.CreateAssessment(request, CurrentUserId, ClientIp), StatusCodes.Status201Created);
    }

    [HttpGet("loan/{loanId:int}")]
    public IActionResult GetAssessments(int loanId)
    {
        return FromResult(_assessmentRepository.GetAssessments(loanId));
    }
}
