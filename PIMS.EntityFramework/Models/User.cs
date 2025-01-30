namespace PIMS.EntityFramework.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Store hash, NOT plain text
        public string Role { get; set; } // "Admin" or "User"
    }
}