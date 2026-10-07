using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;
using ProjectLoanManagement.Repository.Security;

namespace ProjectLoanManagement.Repository.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(DbHelper db, ILogger<UserRepository> logger) : base(db, logger)
    {
    }

    public ResultSet ValidateLogin(string userName, string password, string ipAddress)
    {
        return Execute("AUTH", "Login failed due to a server error.", () =>
        {
            LoginRecord record = GetLoginRecord(userName);

            if (record == null)
            {
                // Same PBKDF2 cost as a real check, so timing does not reveal whether the username exists
                PasswordHasher.Verify(password, PasswordHasher.DummyHash);
                RecordAttempt(userName, null, false, ipAddress);
                return ResultSet.Failure("Invalid username or password.", "AUTH_401");
            }

            User user = record.User;

            if (!user.IsActive)
            {
                RecordAttempt(userName, user.UserId, false, ipAddress);
                return ResultSet.Failure("Your account is inactive. Please contact the administrator.", "AUTH_403");
            }

            if (user.FailedLoginCount >= record.MaxFailedLogins)
            {
                RecordAttempt(userName, user.UserId, false, ipAddress);
                return ResultSet.Failure("Your account is locked after too many failed attempts. Please contact the administrator.", "AUTH_423");
            }

            if (!PasswordHasher.Verify(password, user.PasswordHash))
            {
                RecordAttempt(userName, user.UserId, false, ipAddress);
                return ResultSet.Failure("Invalid username or password.", "AUTH_401");
            }

            RecordAttempt(userName, user.UserId, true, ipAddress);
            user.PasswordHash = null;
            return ResultSet.Success(user, "Login successful");
        });
    }

    public ResultSet CreateUser(CreateUserRequest request, int createdBy, string ipAddress)
    {
        return Execute("USER", "Unable to create the user.", () =>
        {
            string passwordHash = PasswordHasher.Hash(request.Password);

            object newId = Db.Scalar("dbo.sp_CreateUser", p =>
            {
                p.AddNVarChar("@UserName", request.UserName, 50);
                p.AddNVarChar("@PasswordHash", passwordHash, 256);
                p.AddNVarChar("@FullName", request.FullName, 150);
                p.AddNVarChar("@Email", request.Email, 150);
                p.AddNVarChar("@Role", request.Role, 20);
                p.AddInt("@CreatedBy", createdBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            });

            return ResultSet.Success(new { UserId = Convert.ToInt32(newId) }, "User created successfully");
        });
    }

    public ResultSet GetUsers()
    {
        return Execute("USER", "Unable to fetch users.", () =>
        {
            List<User> users = Db.Query("dbo.sp_GetUsers", null, r => r.ReadList(EntityMapper.MapUser));
            return ResultSet.Success(users, "Users fetched successfully");
        });
    }

    public ResultSet GetUserById(int userId)
    {
        return Execute("USER", "Unable to fetch the user.", () =>
        {
            User user = Db.Query("dbo.sp_GetUserById", p => p.AddInt("@UserId", userId),
                r => r.ReadSingle(EntityMapper.MapUser));
            return ResultSet.Success(user, "User fetched successfully");
        });
    }

    public ResultSet SetUserStatus(int userId, bool isActive, int updatedBy, string ipAddress)
    {
        return Execute("USER", "Unable to update the user status.", () =>
        {
            Db.NonQuery("dbo.sp_SetUserStatus", p =>
            {
                p.AddInt("@UserId", userId);
                p.AddBit("@IsActive", isActive);
                p.AddInt("@UpdatedBy", updatedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            });

            return ResultSet.Success(null, isActive ? "User activated (lockout cleared)" : "User deactivated");
        });
    }

    public ResultSet ChangePassword(int userId, string userName, ChangePasswordRequest request, string ipAddress)
    {
        return Execute("AUTH", "Unable to change the password.", () =>
        {
            LoginRecord record = GetLoginRecord(userName);
            if (record == null || record.User.UserId != userId)
            {
                return ResultSet.Failure("User not found.", "AUTH_404");
            }

            if (!PasswordHasher.Verify(request.CurrentPassword, record.User.PasswordHash))
            {
                return ResultSet.Failure("Current password is incorrect.", "AUTH_400");
            }

            if (request.CurrentPassword == request.NewPassword)
            {
                return ResultSet.Failure("The new password must be different from the current password.", "AUTH_400");
            }

            string newHash = PasswordHasher.Hash(request.NewPassword);
            Db.NonQuery("dbo.sp_ChangePassword", p =>
            {
                p.AddInt("@UserId", userId);
                p.AddNVarChar("@NewPasswordHash", newHash, 256);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            });

            return ResultSet.Success(null, "Password changed successfully");
        });
    }

    private LoginRecord GetLoginRecord(string userName)
    {
        return Db.Query("dbo.sp_Login", p => p.AddNVarChar("@UserName", userName, 50), r =>
        {
            if (!r.Read())
            {
                return null;
            }

            User user = EntityMapper.MapUser(r);
            user.PasswordHash = r.Field<string>("PasswordHash");
            return new LoginRecord { User = user, MaxFailedLogins = r.Field<int>("MaxFailedLogins") };
        });
    }

    private void RecordAttempt(string userName, int? userId, bool isSuccess, string ipAddress)
    {
        Db.NonQuery("dbo.sp_RecordLoginAttempt", p =>
        {
            p.AddNVarChar("@UserName", userName, 50);
            p.AddInt("@UserId", userId);
            p.AddBit("@IsSuccess", isSuccess);
            p.AddVarChar("@IPAddress", ipAddress, 45);
        });
    }

    private sealed class LoginRecord
    {
        public User User { get; set; }
        public int MaxFailedLogins { get; set; }
    }
}
