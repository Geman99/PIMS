using System.ComponentModel.DataAnnotations;

namespace PIMS.Model.DTO
{
    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // Get password from DTO

        [Required]
        public string Role { get; set; }
    }
}