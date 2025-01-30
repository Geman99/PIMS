namespace PIMS.Model
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Only for registration (not stored)
        public string Role { get; set; }
    }
}