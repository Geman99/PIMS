using System.ComponentModel.DataAnnotations;

namespace PIMS.Model.DTO
{
    public class UserUpdateDto
    {
        [Required]
        public string Username { get; set; }

        public string Password { get; set; } // Optional: Only update if provided

        [Required]
        public string Role { get; set; }
    }
}