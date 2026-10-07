using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.API.Services;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

[Route("api/customer")]
[Authorize]
public class CustomerController : BaseApiController
{
    private readonly ICustomerRepository _customerRepository;
    private readonly SensitiveDataProtector _protector;

    public CustomerController(ICustomerRepository customerRepository, SensitiveDataProtector protector)
    {
        _customerRepository = customerRepository;
        _protector = protector;
    }

    /// <summary>Create a customer. The Aadhaar number is masked and hashed here and never stored in full.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.LoanDesk)]
    public IActionResult CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        string aadhaarMasked = SensitiveDataProtector.MaskAadhaar(request.AadhaarNo);
        byte[] aadhaarHash = _protector.HashAadhaar(request.AadhaarNo);

        ResultSet result = _customerRepository.CreateCustomer(request, aadhaarMasked, aadhaarHash, CurrentUserId, ClientIp);
        return FromResult(result, StatusCodes.Status201Created);
    }

    /// <summary>Search by name, mobile, customer code or PAN.</summary>
    [HttpGet]
    [Authorize(Roles = Roles.Staff)]
    public IActionResult GetCustomers([FromQuery] string search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        return FromResult(_customerRepository.GetCustomers(search, pageNumber, pageSize));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Staff)]
    public IActionResult GetCustomer(int id)
    {
        return FromResult(_customerRepository.GetCustomerById(id));
    }

    /// <summary>The logged-in customer's own profile.</summary>
    [HttpGet("me")]
    [Authorize(Roles = Roles.Customer)]
    public IActionResult GetMyProfile()
    {
        return FromResult(_customerRepository.GetCustomerByUserId(CurrentUserId));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.LoanDesk)]
    public IActionResult UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
    {
        return FromResult(_customerRepository.UpdateCustomer(id, request, CurrentUserId, ClientIp));
    }
}
