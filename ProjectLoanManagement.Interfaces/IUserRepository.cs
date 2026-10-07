using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface IUserRepository
{
    /// <summary>Checks the password and lockout rules. Data = User on success.</summary>
    ResultSet ValidateLogin(string userName, string password, string ipAddress);

    ResultSet CreateUser(CreateUserRequest request, int createdBy, string ipAddress);

    ResultSet GetUsers();

    ResultSet GetUserById(int userId);

    ResultSet SetUserStatus(int userId, bool isActive, int updatedBy, string ipAddress);

    ResultSet ChangePassword(int userId, string userName, ChangePasswordRequest request, string ipAddress);
}
