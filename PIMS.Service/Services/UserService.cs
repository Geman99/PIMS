using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIMS.EntityFramework.Models;
using Microsoft.AspNetCore.Identity;

namespace PIMS.Services
{
    public class UserService : IUserService
    {
        private readonly PimsDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher; // For password hashing
        private readonly ILogger<UserService> _logger;

        public UserService(PimsDbContext context, IPasswordHasher<User> passwordHasher, ILogger<UserService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return verificationResult == PasswordVerificationResult.Success ? user : new User();
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId) ?? new User();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Hash the password before storing
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash); // Hash the password
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.Username = user.Username;
            existingUser.Role = user.Role;

            if (!string.IsNullOrEmpty(user.PasswordHash)) // Only update password if provided
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, user.PasswordHash);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}