using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.API.Services;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>
/// Shared helpers for every controller:
///  - who is calling (from the JWT claims)
///  - converting a ResultSet into the right HTTP status code
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected int CurrentUserId
    {
        get { return int.TryParse(User.FindFirst(JwtTokenService.UserIdClaim)?.Value, out int id) ? id : 0; }
    }

    protected string CurrentUserName
    {
        get { return User.FindFirst(JwtTokenService.NameClaim)?.Value; }
    }

    protected bool IsCustomer
    {
        get { return User.IsInRole(Roles.Customer); }
    }

    protected string ClientIp
    {
        get { return HttpContext.Connection.RemoteIpAddress?.ToString(); }
    }

    /// <summary>
    /// Success -> 200 (or the given status). Failure -> the number at the end of ErrorCode
    /// (LOAN_404 -> 404, KYC_409 -> 409); anything else -> 400. The body is always the ResultSet.
    /// </summary>
    protected IActionResult FromResult(ResultSet result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.Status)
        {
            return StatusCode(successStatusCode, result);
        }

        return StatusCode(StatusFromErrorCode(result.ErrorCode), result);
    }

    /// <summary>Customers may only see their own loans; staff see everything. Returns null when access is allowed.</summary>
    protected IActionResult DenyIfNotOwnLoan(ILoanRepository loanRepository, int loanId)
    {
        if (!IsCustomer)
        {
            return null;
        }

        ResultSet access = loanRepository.CheckLoanAccess(loanId, CurrentUserId);
        return access.Status ? null : FromResult(access);
    }

    private static int StatusFromErrorCode(string errorCode)
    {
        if (!string.IsNullOrEmpty(errorCode))
        {
            int separator = errorCode.LastIndexOf('_');
            if (separator >= 0 && int.TryParse(errorCode.Substring(separator + 1), out int status) && status >= 400 && status <= 599)
            {
                return status;
            }
        }

        return StatusCodes.Status400BadRequest;
    }
}
