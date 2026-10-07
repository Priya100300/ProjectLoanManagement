using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class CustomerRepository : RepositoryBase, ICustomerRepository
{
    public CustomerRepository(DbHelper db, ILogger<CustomerRepository> logger) : base(db, logger)
    {
    }

    public ResultSet CreateCustomer(CreateCustomerRequest request, string aadhaarMasked, byte[] aadhaarHash, int createdBy, string ipAddress)
    {
        return Execute("CUST", "Unable to create the customer.", () =>
        {
            Customer customer = Db.Query("dbo.sp_CreateCustomer", p =>
            {
                p.AddInt("@UserId", request.UserId);
                p.AddNVarChar("@FullName", request.FullName, 150);
                p.AddVarChar("@MobileNo", request.MobileNo, 10);
                p.AddNVarChar("@Email", request.Email, 150);
                p.AddNVarChar("@Address", request.Address, 300);
                p.AddNVarChar("@City", request.City, 100);
                p.AddNVarChar("@State", request.State, 100);
                p.AddChar("@PinCode", request.PinCode, 6);
                p.AddChar("@PAN", request.PAN, 10);
                p.AddVarChar("@AadhaarNoMasked", aadhaarMasked, 14);
                p.AddVarBinary("@AadhaarHash", aadhaarHash, 32);
                p.AddDate("@DateOfBirth", request.DateOfBirth);
                p.AddInt("@CreatedBy", createdBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapCustomer));

            return ResultSet.Success(customer, "Customer created successfully");
        });
    }

    public ResultSet GetCustomers(string search, int pageNumber, int pageSize)
    {
        return Execute("CUST", "Unable to fetch customers.", () =>
        {
            PagedResult<Customer> page = Db.Query("dbo.sp_GetCustomer", p =>
            {
                p.AddNVarChar("@Search", search, 100);
                p.AddInt("@PageNumber", pageNumber);
                p.AddInt("@PageSize", pageSize);
            }, r => EntityMapper.ReadPaged(r, EntityMapper.MapCustomer, pageNumber, pageSize));

            return ResultSet.Success(page, "Customers fetched successfully");
        });
    }

    public ResultSet GetCustomerById(int customerId)
    {
        return Execute("CUST", "Unable to fetch the customer.", () =>
        {
            Customer customer = Db.Query("dbo.sp_GetCustomerById", p => p.AddInt("@CustomerId", customerId),
                r => r.ReadSingle(EntityMapper.MapCustomer));
            return ResultSet.Success(customer, "Customer fetched successfully");
        });
    }

    public ResultSet GetCustomerByUserId(int userId)
    {
        return Execute("CUST", "Unable to fetch the customer profile.", () =>
        {
            Customer customer = Db.Query("dbo.sp_GetCustomerByUserId", p => p.AddInt("@UserId", userId),
                r => r.ReadSingle(EntityMapper.MapCustomer));
            return ResultSet.Success(customer, "Customer profile fetched successfully");
        });
    }

    public ResultSet UpdateCustomer(int customerId, UpdateCustomerRequest request, int updatedBy, string ipAddress)
    {
        return Execute("CUST", "Unable to update the customer.", () =>
        {
            Customer customer = Db.Query("dbo.sp_UpdateCustomer", p =>
            {
                p.AddInt("@CustomerId", customerId);
                p.AddNVarChar("@FullName", request.FullName, 150);
                p.AddVarChar("@MobileNo", request.MobileNo, 10);
                p.AddNVarChar("@Email", request.Email, 150);
                p.AddNVarChar("@Address", request.Address, 300);
                p.AddNVarChar("@City", request.City, 100);
                p.AddNVarChar("@State", request.State, 100);
                p.AddChar("@PinCode", request.PinCode, 6);
                p.AddInt("@UpdatedBy", updatedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapCustomer));

            return ResultSet.Success(customer, "Customer updated successfully");
        });
    }
}
