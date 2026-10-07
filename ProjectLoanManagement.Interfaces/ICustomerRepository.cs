using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface ICustomerRepository
{
    /// <summary>The raw Aadhaar number never reaches this layer - only its mask and keyed hash.</summary>
    ResultSet CreateCustomer(CreateCustomerRequest request, string aadhaarMasked, byte[] aadhaarHash, int createdBy, string ipAddress);

    ResultSet GetCustomers(string search, int pageNumber, int pageSize);

    ResultSet GetCustomerById(int customerId);

    ResultSet GetCustomerByUserId(int userId);

    ResultSet UpdateCustomer(int customerId, UpdateCustomerRequest request, int updatedBy, string ipAddress);
}
