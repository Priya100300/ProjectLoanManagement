using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>User management (Admin only).</summary>
[Route("api/user")]
[Authorize(Roles = Roles.Admin)]
public class UserController : BaseApiController
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        return FromResult(_userRepository.GetUsers());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetUser(int id)
    {
        return FromResult(_userRepository.GetUserById(id));
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        return FromResult(_userRepository.CreateUser(request, CurrentUserId, ClientIp), StatusCodes.Status201Created);
    }

    /// <summary>Activate (also clears a login lockout) or deactivate a user.</summary>
    [HttpPut("{id:int}/status")]
    public IActionResult SetStatus(int id, [FromBody] UserStatusRequest request)
    {
        return FromResult(_userRepository.SetUserStatus(id, request.IsActive.Value, CurrentUserId, ClientIp));
    }
}
