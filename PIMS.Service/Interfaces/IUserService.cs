using PIMS.EntityFramework.Models;
using PIMS.Model;

namespace PIMS.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<User> CreateUserAsync(User user); // Admin function

        Task<List<User>> GetAllUsersAsync(); // Admin function

        Task<bool> UpdateUserAsync(Guid id, User user);  // Admin function

        Task<bool> DeleteUserAsync(Guid id);  // Admin function
    }
}