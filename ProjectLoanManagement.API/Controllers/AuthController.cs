using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.API.Services;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>Login and password management.</summary>
[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _tokenService;

    public AuthController(IUserRepository userRepository, JwtTokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    /// <summary>Log in and receive a JWT. Use it in Swagger's Authorize button as "Bearer {token}".</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        ResultSet result = _userRepository.ValidateLogin(request.UserName, request.Password, ClientIp);
        if (!result.Status)
        {
            return FromResult(result);
        }

        LoginResponse response = _tokenService.CreateToken((User)result.Data);
        return FromResult(ResultSet.Success(response, "Login successful"));
    }

    /// <summary>Change your own password.</summary>
    [Authorize]
    [HttpPost("change-password")]
    public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
    {
        return FromResult(_userRepository.ChangePassword(CurrentUserId, CurrentUserName, request, ClientIp));
    }
}
